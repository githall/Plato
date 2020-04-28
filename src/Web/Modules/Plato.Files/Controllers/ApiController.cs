using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using Plato.Files.Attributes;
using Plato.Files.Services;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.Files.Models;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Features.Abstractions;
using PlatoCore.Http.Abstractions;
using Microsoft.AspNetCore.Authorization;
using PlatoCore.Security.Abstractions;

namespace Plato.Files.Controllers
{

    public class ApiController : BaseWebApiController
    {

        public const string ModuleId = "Plato.Files";      

        private readonly IHttpMultiPartRequestHandler _multiPartRequestHandler;        
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ApiController> _logger;
        private readonly IFeatureFacade _featureFacade;
        private readonly IFileValidator _fileValidator;
        private readonly IFileStore<File> _fileStore;
        private readonly IFileManager _fileManager;

        public IHtmlLocalizer T { get; }

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public ApiController(     
            IHttpMultiPartRequestHandler multiPartRequestHandler,
            IFileStore<File> attachmentStore,
            IAuthorizationService authorizationService,
            IFileValidator attachmentValidator,
            ILogger<ApiController> logger,
            IHtmlLocalizer htmlLocalizer,
            IFeatureFacade featureFacade,
            IFileManager fileManager)
        {

            _multiPartRequestHandler = multiPartRequestHandler;
            _authorizationService = authorizationService;           
            _fileValidator = attachmentValidator;            
            _featureFacade = featureFacade;
            _fileStore = attachmentStore;
            _fileManager = fileManager;
            _logger = logger;

            T = htmlLocalizer;

        }

        // -----------
        // Post
        // -----------

        // Request limits for files are enforced by Plato so set MVCs request limits 
        // for this action to some arbitrary high value, in this case 1 gigabyte

        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> Post()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddFiles))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await base.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);

            // Ensure the feature exists
            if (feature == null)
            {
                throw new Exception($"A feature named \"{ModuleId}\" could not be found!");
            }

            // Validate & process multi-part request
            // -------------------

            var result = await _multiPartRequestHandler.ProcessAsync(Request);

            // Return any errors parsing the multi-part request
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return BadRequest(error);
                }
            }

            // Build file
            // -------------------

            var md5 = result.Response.ContentBytes?.ToMD5().ToHex() ?? string.Empty;
            var newFile = new File
            {
                FeatureId = feature.Id,
                Name = result.Response.Name,
                Extension = result.Response.Extension,
                ContentType = result.Response.ContentType,
                ContentLength = result.Response.ContentLength,
                ContentBlob = result.Response.ContentBytes,                
                ContentCheckSum = md5,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.Now
            };

            // Validate file
            // -------------------

            var output = new List<UploadResult>();

            var validationResult = await _fileValidator.ValidateAsync(newFile);
            if (validationResult.Succeeded)
            {

                // Create file
                var fileResult = await _fileManager.CreateAsync(newFile);

                // Build friendly results
                if (fileResult.Succeeded)
                {
                    output.Add(new UploadResult()
                    {
                        Id = fileResult.Response.Id,
                        Name = fileResult.Response.Name,
                        ContentType = fileResult.Response.ContentType,
                        ContentLength = fileResult.Response.ContentLength,
                        FriendlySize = fileResult.Response.ContentLength.ToFriendlyFileSize()
                    });
                }
                else
                {
                    foreach (var error in fileResult.Errors)
                    {
                        output.Add(new UploadResult()
                        {
                            Name = result.Response.Name,
                            Error = error.Description
                        });
                    }
                }

            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    output.Add(new UploadResult()
                    {
                        Name = result.Response.Name,
                        Error = error.Description
                    });
                }
            }

            return base.Result(output);

        }

        // -----------
        // Update
        // -----------

        // Request limits for files are enforced by Plato so set MVCs request limits 
        // for this action to some arbitrary high value, in this case 1 gigabyte

        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> Put()
        {
            
            var id = string.Empty; 
            if (Request.Query.ContainsKey("id"))
            {
                id = Request.Query["id"];
            }         

            var ok = int.TryParse(id, out var fileId);

            if (!ok)
            {
                return NotFound();
            }

            // Get existing file
            var file = await _fileStore.GetByIdAsync(fileId);
            
            // Ensure the file exists
            if (file == null)
            {
                return NotFound();
            }

            // Get authenticated user
            var user = await base.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Determine permission to check against
            var postPermission = file.CreatedUserId == user.Id
                 ? Permissions.EditOwnFiles
                 : Permissions.EditAnyFile;

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, postPermission))
            {
                return Unauthorized();
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

            // Build file
            // -------------------

            var md5 = result.Response.ContentBytes?.ToMD5().ToHex() ?? string.Empty;
            var newFile = new File
            {
                Id = file.Id,
                FeatureId = file.Id,
                Name = result.Response.Name,
                Extension = result.Response.Extension,
                ContentType = result.Response.ContentType,
                ContentLength = result.Response.ContentLength,
                ContentBlob = result.Response.ContentBytes,
                ContentCheckSum = md5,
                CreatedUserId = file.CreatedUserId,
                CreatedDate = file.CreatedDate,
                ModifiedUserId = user.Id,
                ModifiedDate = DateTimeOffset.Now
            };

            // Validate file
            // -------------------

            var output = new List<UploadResult>();

            var validationResult = await _fileValidator.ValidateAsync(newFile);
            if (validationResult.Succeeded)
            {

                // Update file
                var fileResult = await _fileManager.UpdateAsync(newFile);

                // Build friendly result
                if (fileResult.Succeeded)
                {
                    output.Add(new UploadResult()
                    {
                        Id = fileResult.Response.Id,
                        Name = fileResult.Response.Name,
                        ContentType = fileResult.Response.ContentType,
                        ContentLength = fileResult.Response.ContentLength,
                        FriendlySize = fileResult.Response.ContentLength.ToFriendlyFileSize()
                    });
                } 
                else
                {
                    foreach (var error in fileResult.Errors)
                    {
                        output.Add(new UploadResult()
                        {
                            Name = result.Response.Name,
                            Error = error.Description
                        });
                    }
                }

            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    output.Add(new UploadResult()
                    {
                        Name = result.Response.Name,
                        Error = error.Description
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
            var attachment = await _fileStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (attachment == null)
            {
                return NotFound();
            }        

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteOwnFiles))
            {
                return Unauthorized();
            }

            // Delete attachment
            var success = await _fileStore.DeleteAsync(attachment);

            // Return result
            return base.Result(success);

        }

    }

}
