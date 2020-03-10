using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using Plato.Attachments.Attributes;
using Plato.Attachments.Services;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.Attachments.Models;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Features.Abstractions;
using PlatoCore.Net.Abstractions;
using Microsoft.AspNetCore.Authorization;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Attachments.Controllers
{

    public class ApiController : BaseWebApiController
    {

        public const string ModuleId = "Plato.Articles.Attachments";
        public const string GuidKey = "guid";        

        private readonly IHttpMultiPartRequestHandler _multiPartRequestHandler;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAttachmentValidator _attachmentValidator;
        private readonly ILogger<ApiController> _logger;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public ApiController(     
            IHttpMultiPartRequestHandler multiPartRequestHandler,
            IAttachmentStore<Attachment> attachmentStore,
            IAuthorizationService authorizationService,
            IAttachmentValidator attachmentValidator,
            ILogger<ApiController> logger,
            IHtmlLocalizer htmlLocalizer,
            IFeatureFacade featureFacade)
        {            
            _multiPartRequestHandler = multiPartRequestHandler;
            _authorizationService = authorizationService;           
            _attachmentValidator = attachmentValidator;
            _attachmentStore = attachmentStore;
            _featureFacade = featureFacade;
            _logger = logger;

            T = htmlLocalizer;

        }

        // -----------
        // Post
        // -----------

        // Request limits for attachments are enforced by Plato so set MVCs request limits 
        // for this action to some arbitrary high value, in this case 1 gigabyte

        [HttpPost, DisableFormValueModelBinding, ValidateClientAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> Post()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.PostArticleAttachments))
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
            var guid = GetGuid();
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

            // Build attachment
            // -------------------

            var md5 = result.Response.ContentBytes?.ToMD5().ToHex() ?? string.Empty;
            var attachment = new Attachment
            {
                FeatureId = feature.Id,
                Name = result.Response.Name,
                ContentType = result.Response.ContentType,
                ContentLength = result.Response.ContentLength,
                ContentBlob = result.Response.ContentBytes,
                ContentGuid = guid,
                ContentCheckSum = md5,
                CreatedUserId = user.Id
            };

            // Validate attachment
            // -------------------

            var output = new List<UploadResult>();

            var validationResult = await _attachmentValidator.ValidateAsync(attachment);
            if (validationResult.Succeeded)
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
                        ContentType = attachment.ContentType,
                        ContentLength = attachment.ContentLength,
                        FriendlySize = attachment.ContentLength.ToFriendlyFileSize()
                    });
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
            var attachment = await _attachmentStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (attachment == null)
            {
                return NotFound();
            }

            // Get current permission based on attachment ownership
            var deletePermission = attachment.CreatedUserId == user.Id
                ? Permissions.DeleteOwnArticleAttachments
                : Permissions.DeleteAnyArticleAttachments;

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, deletePermission))
            {
                return Unauthorized();
            }

            // Delete attachment
            var success = await _attachmentStore.DeleteAsync(attachment);

            // Return result
            return base.Result(success);

        }

        // -----------------

        string GetGuid()
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
