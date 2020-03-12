using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PlatoCore.Stores.Abstractions.Files;
using Plato.Media.Stores;
using Microsoft.Net.Http.Headers;

namespace Plato.Media.Controllers
{

    public class MediaController : Controller
    {

        private static string _pathToEmptyImage;

        private readonly IMediaStore<Models.Media> _mediaStore;
        private readonly IFileStore _fileStore;

        public MediaController(    
            IMediaStore<Models.Media> mediaStore,
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
            Response.Clear();

            if ((media != null) && (media.ContentLength >= 0))
            {
                Response.ContentType = media.ContentType;
                Response.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + media.Name + "\"");
                Response.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)media.ContentLength));
                Response.Headers.Add(HeaderNames.CacheControl, "public,max-age=7776000"); // 7776000 = 90 days
                await Response.Body.WriteAsync(media.ContentBlob, 0, (int)media.ContentLength);
            }
            else
            {
                var fileBytes = await _fileStore.GetFileBytesAsync(_pathToEmptyImage);
                if (fileBytes != null)
                {
                    Response.ContentType = "image/png";
                    Response.Headers.Add(HeaderNames.ContentDisposition, "filename=\"empty.png\"");
                    Response.Headers.Add(HeaderNames.ContentLength, Convert.ToString((int)fileBytes.Length));
                    Response.Headers.Add(HeaderNames.CacheControl, "public,max-age=7776000"); // 7776000 = 90 days
                    await Response.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
                }
            }

        }

    }

}