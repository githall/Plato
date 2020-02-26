using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PlatoCore.Stores.Abstractions.Files;
using Plato.Attachments.Stores;
using Microsoft.Net.Http.Headers;

namespace Plato.Attachments.Controllers
{

    public class AttachmentController : Controller
    {

        private static string _pathToEmptyImage;

        private readonly IAttachmentStore<Models.Attachment> _mediaStore;
        private readonly IFileStore _fileStore;

        public AttachmentController(    
            IAttachmentStore<Models.Attachment> mediaStore,
            IHostEnvironment hostEnvironment,
            IFileStore fileStore)
        {

            _mediaStore = mediaStore;       
            _fileStore = fileStore;

            if (_pathToEmptyImage == null)
            {
                _pathToEmptyImage = _fileStore.Combine(hostEnvironment.ContentRootPath,
                    "wwwroot",
                    "images",
                    "photo.png");
            }

        }

        // ----------
        // Serve
        // ----------

        [HttpGet, AllowAnonymous]
        public async Task Serve(int id)
        {

            var media = await _mediaStore.GetByIdAsync(id);
            var r = Response;
            r.Clear();

            if ((media != null) && (media.ContentLength >= 0))
            {
                r.ContentType = media.ContentType;
                r.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + media.Name + "\"");
                r.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)media.ContentLength));
                r.Headers.Add(HeaderNames.CacheControl, "public,max-age=7776000"); // 7776000 = 90 days
                await r.Body.WriteAsync(media.ContentBlob, 0, (int)media.ContentLength);
            }
            else
            {
                var fileBytes = await _fileStore.GetFileBytesAsync(_pathToEmptyImage);
                if (fileBytes != null)
                {
                    r.ContentType = "image/png";
                    r.Headers.Add(HeaderNames.ContentDisposition, "filename=\"empty.png\"");
                    r.Headers.Add(HeaderNames.ContentLength, Convert.ToString((int)fileBytes.Length));
                    r.Headers.Add(HeaderNames.CacheControl, "public,max-age=7776000"); // 7776000 = 90 days
                    await r.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
                }
            }

        }

    }

}