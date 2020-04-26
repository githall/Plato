using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Http.Abstractions;

namespace PlatoCore.Http
{

    public class HttpMultiPartRequestHandler : IHttpMultiPartRequestHandler
    {

        // https://github.com/aspnet/Docs/tree/master/aspnetcore/mvc/models/file-uploads/sample/FileUploadSample

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public async Task<ICommandResult<MultiPartRequestResult>> ProcessAsync(HttpRequest request)
        {

            var result = new CommandResult<MultiPartRequestResult>();

            // Ensure we are dealing with a multipart request
            if (!HttpMultiPartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                return result.Failed($"Expected a multipart request, but got {request.ContentType}");
            }

            // Used to accumulate all the form url
            // encoded key value pairs in the request.
            var formAccumulator = new KeyValueAccumulator();
            var boundary = HttpMultiPartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, request.Body);

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
                    if (HttpMultiPartRequestHelper.HasFileContentDisposition(contentDisposition))
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
                    else if (HttpMultiPartRequestHelper.HasFormDataContentDisposition(contentDisposition))
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
                return result.Failed("Could not construct a byte array for the multipart request.");              
            }

            // Return success
            return result.Success(new MultiPartRequestResult()
            {
                Name = name,
                Extension = extension,
                ContentType = contentType,
                ContentLength = contentLength,
                ContentBytes = bytes
            });

        }

        // ----------------------

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

    public class MultiPartRequestResult 
    {

        public string Name { get; set; }

        public string Extension { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public byte[] ContentBytes { get; set; }

    }
}
