using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using PlatoCore.Abstractions.Extensions;
using Plato.Attachments.Attributes;
using Plato.Attachments.Services;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.Attachments.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Attachments.Controllers
{

    // https://github.com/aspnet/Docs/tree/master/aspnetcore/mvc/models/file-uploads/sample/FileUploadSample

    public class ApiController : BaseWebApiController
    {

        public const string FeatureIdKey = "featureId";
        public const string GuidKey = "guid";
    
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

        private readonly IAttachmentInfoStore<AttachmentInfo> _attachmentInfoStore;
        private readonly IAttachmentOptionsFactory _attachmentOptionsFactory;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly ILogger<ApiController> _logger;

        public IHtmlLocalizer T { get; }

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public ApiController(
            IAttachmentInfoStore<AttachmentInfo> attachmentInfoStore,
            IAttachmentOptionsFactory attachmentOptionsFactory,
            IAttachmentStore<Attachment> attachmentStore,
            ILogger<ApiController> logger,
            IHtmlLocalizer htmlLocalizer)
        {
            _attachmentOptionsFactory = attachmentOptionsFactory;
            _attachmentInfoStore = attachmentInfoStore;
            _attachmentStore = attachmentStore;
            _logger = logger;

            T = htmlLocalizer;

        }

        // -----------
        // Post
        // -----------
            
        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> Post()
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // TODO: Add upload permission checks
            // ----------------------

            // Validate temporary global unique identifier

            if (!Request.Query.ContainsKey(GuidKey))
            {
                return BadRequest($"A \"{GuidKey}\" query string parameter is required!");
            }

            var ok = int.TryParse(Request.Query[FeatureIdKey], out var featureId);
            var guid = Request.Query[GuidKey];
            if (string.IsNullOrEmpty(guid))
            {
                return BadRequest($"The \"{GuidKey}\" query string parameter is empty!");
            }

            // Ensure we are dealing with a multipart request
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            // Used to accumulate all the form url
            // encoded key value pairs in the request.
            var formAccumulator = new KeyValueAccumulator();
            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            // Store file information
            var name = string.Empty;
            var extension = string.Empty;
            var contentType = string.Empty;
            long contentLength = 0;
            byte[] bytes = null;

            // Prase multipart sections
            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {

                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);            
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {

                        name = contentDisposition.FileName.ToString();
                        extension = Path.GetExtension(name);
                        contentType = section.ContentType;

                        // Create an in-memory stream to get the bytes and length
                        using (var ms = new MemoryStream())
                        {

                            // Read the seciton into our memory stream
                            await section.Body.CopyToAsync(ms);

                            // get bytes and length
                            bytes = ms.ToByteArray();
                            contentLength = ms.Length;

                        }

                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {

                        // Content-Disposition: form-data; name="key" value
                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.

                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).ToString();
                        var encoding = GetEncoding(section);

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Get btye array from memory stream for storage         
    
            if (bytes == null)
            {
                return BadRequest($"Could not construct a byte array for the uploaded file.");
            }

            // At this stage everything was OK with the file, build the attachment
            // -------------------

            var attachment = new Attachment
            {
                FeatureId = featureId,
                Name = name,
                ContentType = contentType,
                ContentLength = contentLength,
                ContentBlob = bytes,
                ContentGuid = guid,
                ContentCheckSum = bytes?.ToMD5().ToHex() ?? string.Empty,
                CreatedUserId = user.Id
            };

            // Validate the attachment
            // -------------------

            bool validExtension = false, 
                validSize = false, 
                validSpace = false;

            long spaceRemaining = 0;

            // Get options & info
            var options = await _attachmentOptionsFactory.GetOptionsAsync(user);

            if (options != null)
            {

                // Ensure content is below or equal to our max file size
                validSize = attachment.ContentLength <= options.MaxFileSize;

                // Ensure extension is allowed
                if (!string.IsNullOrEmpty(extension))
                {
                    foreach (var allowedExtension in options.AllowedExtensions)
                    {
                        if (extension.Equals($".{allowedExtension}", StringComparison.OrdinalIgnoreCase))
                        {
                            validExtension = true;
                        }
                    }
                }

                // Ensure the upload would not exceed available space
                var info = await _attachmentInfoStore.GetByUserIdAsync(user?.Id ?? 0);
                if (info != null)
                {
                    if ((info.Length + attachment.ContentLength) <= options.AvailableSpace)
                    {
                        validSpace = true;
                    }
                    spaceRemaining = options.AvailableSpace - info.Length;
                    if (spaceRemaining < 0)
                        spaceRemaining = 0;
                }

            }

            // Build results
            // -------------------

            var output = new List<UploadResult>();

            // Validation OK?
            if (validExtension && validSize && validSpace)
            {

                // Store attachment
                attachment = await _attachmentStore.CreateAsync(attachment);

                // Build friendly result
                if (attachment != null)
                {
                    output.Add(new UploadResult()
                    {
                        Id = attachment.Id,
                        Name = attachment.Name,
                        FriendlySize = attachment.ContentLength.ToFriendlyFileSize(),
                        IsImage = IsContentTypeSupported(attachment.ContentType, SupportedImageContentTypes),
                        IsBinary = IsContentTypeSupported(attachment.ContentType, SupportedBinaryContentTypes),
                    });
                }

            }
            else
            {

                // File to big
                if (!validSize)
                {
                    var text = T["The file is {0} which exceeds your configured maximum allowed file size of {1}."];     
                    output.Add(new UploadResult()
                    {
                        Name = name,
                        Error = string.Format(
                                text.Value,                          
                                contentLength.ToFriendlyFileSize(),
                                options.MaxFileSize.ToFriendlyFileSize())
                    }); 
                }

                // Invalid extension
                if (!validExtension)
                {
                    var allowedExtensions = string.Join(",", options.AllowedExtensions.Select(e => e));
                    if (!string.IsNullOrEmpty(allowedExtensions))
                    {
                        // Our extension does not appear within the allowed extensions white list
                        var text = T["The file is not an allowed type. You are allowed to attach the following types:- {0}"];
                        output.Add(new UploadResult()
                        {
                            Name = name,
                            Error = string.Format(
                                    text.Value,                                  
                                    allowedExtensions.Replace(",", ", "))
                        });
                    } 
                    else
                    {
                        // We don't have any configured allowed extensions
                        var text = T["The file is not an allowed type. No allowed file extensions have been configured for your account."];
                        output.Add(new UploadResult()
                        {
                            Name = name,
                            Error = text.Value
                        });
                    }
                }

                // No space available
                if (!validSpace)
                {
                    var text = T["Not enough free space. You only have {0} of free space available but the upload was {1}."];
                    output.Add(new UploadResult()
                    {
                        Name = name,
                        Error = string.Format(
                                text.Value,                             
                                spaceRemaining.ToFriendlyFileSize(),
                                attachment.ContentLength.ToFriendlyFileSize())
                    });
                }

            }

            return base.Result(output);

        }

        // -----------
        // Delete
        // -----------

        [HttpPost, ValidateClientAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
     
            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Get authenticated user
            var user = await base.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Get attachment
            var attachment = await _attachmentStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (attachment == null)
            {
                return NotFound();
            }

            // Delete attachment
            var success = await _attachmentStore.DeleteAsync(attachment);

            // Return result
            return base.Result(success);

        }

        // -----------------

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

        Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored.
            // UTF-8 will succeed in most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

    }

}
