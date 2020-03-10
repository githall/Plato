using System;
using System.Linq;
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

        private readonly IAttachmentInfoStore<AttachmentInfo> _attachmentInfoStore;
        private readonly IAttachmentOptionsFactory _attachmentOptionsFactory;
        private readonly IHttpMultiPartRequestHandler _multiPartRequestHandler;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ApiController> _logger;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        // Get the default form options so that we can use them
        // to set the default limits for request body data
        private readonly FormOptions _defaultFormOptions = new FormOptions();

        public ApiController(
            IAttachmentInfoStore<AttachmentInfo> attachmentInfoStore,
            IAttachmentOptionsFactory attachmentOptionsFactory,
            IHttpMultiPartRequestHandler multiPartRequestHandler,
            IAttachmentStore<Attachment> attachmentStore,
            IAuthorizationService authorizationService,
            ILogger<ApiController> logger,
            IHtmlLocalizer htmlLocalizer,
            IFeatureFacade featureFacade)
        {
            _attachmentOptionsFactory = attachmentOptionsFactory;
            _multiPartRequestHandler = multiPartRequestHandler;
            _authorizationService = authorizationService;
            _attachmentInfoStore = attachmentInfoStore;            
            _attachmentStore = attachmentStore;
            _featureFacade = featureFacade;
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
            var result = await _multiPartRequestHandler.ProcessAsync(Request);

            // Return any errors parsing the multipart request
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return BadRequest(error);
                }
            }

            // At this stage everything was OK with the file, build the attachment
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
                if (!string.IsNullOrEmpty(result.Response.Extension))
                {
                    foreach (var allowedExtension in options.AllowedExtensions)
                    {
                        if (result.Response.Extension.Equals($".{allowedExtension}", StringComparison.OrdinalIgnoreCase))
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
                        ContentType = attachment.ContentType,
                        ContentLength = attachment.ContentLength,
                        FriendlySize = attachment.ContentLength.ToFriendlyFileSize()
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
                        Name = result.Response.Name,
                        Error = string.Format(
                                text.Value,
                                result.Response.ContentLength.ToFriendlyFileSize(),
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
                            Name = result.Response.Name,
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
                            Name = result.Response.Name,
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
                        Name = result.Response.Name,
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
                
                var guid = Request.Query[GuidKey];
                if (!string.IsNullOrEmpty(guid))
                {
                    return guid;
                }
            }

            return string.Empty;

        }

    }

}
