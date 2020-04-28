using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.Models;

namespace Plato.Ideas.Files.Controllers
{

    public class ApiController : BaseWebApiController
    {

        public const string ModuleId = "Plato.Ideas.Files";
        public const string GuidKey = "guid";        

        private readonly IHttpMultiPartRequestHandler _multiPartRequestHandler;
        private readonly IEntityFileStore<EntityFile> _entityFileStore;
        private readonly IAuthorizationService _authorizationService; 
        private readonly IFileValidator _fileValidator;
        private readonly ILogger<ApiController> _logger;
        private readonly IFeatureFacade _featureFacade;
        private readonly IFileStore<File> _fileStore;
        private readonly IFileManager _fileManager;  

        public IHtmlLocalizer T { get; }

        public ApiController(     
            IHttpMultiPartRequestHandler multiPartRequestHandler,
            IEntityFileStore<EntityFile> entityFileStore,
            IAuthorizationService authorizationService,
            IFileValidator attachmentValidator,
            IFileStore<File> attachmentStore,
            ILogger<ApiController> logger,
            IHtmlLocalizer htmlLocalizer,
            IFeatureFacade featureFacade,
            IFileManager fileManager)
        {            
            _multiPartRequestHandler = multiPartRequestHandler;
            _authorizationService = authorizationService;           
            _fileValidator = attachmentValidator;
            _entityFileStore = entityFileStore;
            _featureFacade = featureFacade;
            _fileStore = attachmentStore;
            _fileManager = fileManager;
            _logger = logger;

            T = htmlLocalizer;

        }

        // -----------
        // Post File
        // -----------

        // Request limits for files are enforced by Plato so set MVCs request limits 
        // for this action to some arbitrary high value, in this case 1 gigabyte

        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> Post()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.PostIdeaFiles))
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

            // Validate temporary global unique identifier
            var guid = GetFileContentGuid();
            if (string.IsNullOrEmpty(guid))
            {
                return BadRequest($"The \"{GuidKey}\" query string parameter is empty!");
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
                FeatureId = feature.Id,
                Name = result.Response.Name,
                Extension = result.Response.Extension,
                ContentType = result.Response.ContentType,
                ContentLength = result.Response.ContentLength,
                ContentBlob = result.Response.ContentBytes,
                ContentGuid = guid,
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
        // Delete File
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
            var file = await _fileStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (file == null)
            {
                return NotFound();
            }

            // Get current permission based on attachment ownership
            var deletePermission = file.CreatedUserId == user.Id
                ? Permissions.DeleteOwnIdeaFiles
                : Permissions.DeleteAnyIdeaFile;

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, deletePermission))
            {
                return Unauthorized();
            }

            // Delete file
            var deleteResult = await _fileManager.DeleteAsync(file);

            if (deleteResult.Succeeded)
            {
                // Delete all entity relationships for deleted file
                await _entityFileStore.DeleteByFileIdAsync(deleteResult.Response.Id);
            }

            // Return result
            return base.Result(deleteResult.Succeeded);

        }

        // -----------------

        string GetFileContentGuid()
        {

            if (Request.Query.ContainsKey(GuidKey))
            {           
                if (!string.IsNullOrEmpty(Request.Query[GuidKey]))
                {
                    return Request.Query[GuidKey];
                }
            }

            return string.Empty;

        }

    }

}
