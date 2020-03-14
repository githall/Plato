using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using Plato.Media.Attributes;
using Plato.Media.Stores;
using Plato.Media.ViewModels;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using PlatoCore.Net.Abstractions;

namespace Plato.Media.Controllers
{

    public class StreamingController : BaseWebApiController
    {

        private static readonly string[] SupportedImageContentTypes = new string[]
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/jpg",
            "image/bmp"
        };

        private static readonly string[] SupportedBinaryContentTypes = new string[]
        {
            "text/plain",
            "text/html",
            "application/octet-stream"
        };

        private readonly IHttpMultiPartRequestHandler _multiPartRequestHandler;
        private readonly ILogger<StreamingController> _logger;
        private readonly IMediaStore<Models.Media> _mediaStore;

        public StreamingController(
            IHttpMultiPartRequestHandler multiPartRequestHandler,
            ILogger<StreamingController> logger,
            IMediaStore<Models.Media> mediaStore)
        {
            _multiPartRequestHandler = multiPartRequestHandler;
            _mediaStore = mediaStore;
            _logger = logger;            
        }

        #region "Actions"

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // 524288000 bytes = 500mb
        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [RequestSizeLimit(524288000)]
        public async Task<IActionResult> Upload()
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Validate & process multipart request
            // -------------------

            var result = await _multiPartRequestHandler.ProcessAsync(Request);

            // Return any errors parsing the multipart request
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return BadRequest(error);
                }
            }

            // Build output
            // -------------------

            var output = new List<UploadedFile>();

            // Store media
            var media = await _mediaStore.CreateAsync(new Models.Media
            {
                Name = result.Response.Name,
                ContentType = result.Response.ContentType,
                ContentLength = result.Response.ContentLength,
                ContentBlob = result.Response.ContentBytes,
                CreatedUserId = user.Id
            });

            // Build friendly results
            if (media != null)
            {
                output.Add(new UploadedFile()
                {
                    Id = media.Id,
                    Name = media.Name,
                    FriendlySize = media.ContentLength.ToFriendlyFileSize(),
                    IsImage = IsContentTypeSupported(media.ContentType, SupportedImageContentTypes),
                    IsBinary = IsContentTypeSupported(media.ContentType, SupportedBinaryContentTypes),
                });
            }

            return base.Result(output);

        }

        #endregion

        #region "Private Methods"

        bool IsContentTypeSupported(string contentType, string[] supportedTypes)
        {

            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            foreach (var supportedType in supportedTypes)
            {
                if (contentType.Equals(supportedType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;

        }

        #endregion

    }

}
