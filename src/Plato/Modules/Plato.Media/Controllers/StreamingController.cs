﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Media.Attributes;
using Plato.Media.Filters;
using Plato.Media.Services;
using Plato.Media.Stores;
using Plato.Media.ViewModels;
using Plato.WebApi.Controllers;
using ContentDispositionHeaderValue = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace Plato.Media.Controllers
{
    
    public class UploadedData
    {
        public string FilePath { get; set; }

    }

    public class StreamingController : BaseWebApiController
    {

   
        private readonly ILogger<StreamingController> _logger;
        private readonly IMediaStore<Models.Media> _mediaStore;

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public StreamingController(
            ILogger<StreamingController> logger,
            IMediaStore<Models.Media> mediaStore)
        {
            _logger = logger;
            _mediaStore = mediaStore;
        }

        [HttpGet]
        [GenerateAntiforgeryTokenCookieForAjax]
        public IActionResult Index()
        {
            return View();
        }

        #region snippet1

        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [HttpPost]
        [DisableFormValueModelBinding]
        /*[ValidateAntiForgeryToken]*/
        public async Task<IActionResult> Upload()
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
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
            
            var name = string.Empty;
            var contentType = string.Empty;
            long size = 0;
            var ms = new MemoryStream();

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
           
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        //targetFilePath = Path.GetTempFileName();

                        name = contentDisposition.FileName.ToString();
                        contentType = section.ContentType;
                     
                        if (contentDisposition.Size != null)
                        {
                            size = (long)contentDisposition.Size;
                        }
                        
                        // Read the seciton into our memory stream
                        await section.Body.CopyToAsync(ms);

                        //using (var targetStream = System.IO.File.Create(targetFilePath))
                        //{
                        //    await section.Body.CopyToAsync(targetStream);

                        //    _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        //}
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

            // Bind form data to a model
         
            //var formValueProvider = new FormValueProvider(
            //    BindingSource.Form,
            //    new FormCollection(formAccumulator.GetResults()),
            //    CultureInfo.CurrentCulture);
            
            // Get btye array from stream for storage
            var bytes =  ms.StreamToByteArray();
            if (bytes == null)
            {
                throw new InvalidDataException("A problem occurred with the upload. Please try again.");
            }
            
            var output = new List<UploadedFile>();
            
            // Add media to database
            var media = await _mediaStore.CreateAsync(new Models.Media
            {
                Name = name,
                ContentType = contentType,
                ContentLength = bytes.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedUserId = user.Id,
                ModifiedDate = DateTime.UtcNow
            });

            if (media != null)
            {
                output.Add(new UploadedFile()
                {
                    Id = media.Id,
                    Name = media.Name,
                    Size = media.ContentLength
                });
            }
      
            return base.Result(output);

        }

        #endregion

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
