using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using System.Collections.Generic;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Abstractions.Extensions;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Attachments.Services
{

    public class AttachmentValidator : IAttachmentValidator
    {

        private readonly IAttachmentInfoStore<AttachmentInfo> _attachmentInfoStore;
        private readonly IAttachmentOptionsFactory _attachmentOptionsFactory;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public AttachmentValidator(
            IHtmlLocalizer htmlLocalizer,
            IAttachmentInfoStore<AttachmentInfo> attachmentInfoStore,
            IAttachmentOptionsFactory attachmentOptionsFactory,
            IContextFacade contextFacade)
        {

            _attachmentOptionsFactory = attachmentOptionsFactory;
            _attachmentInfoStore = attachmentInfoStore;
            _contextFacade = contextFacade;

            T = htmlLocalizer;

        }

        public async Task<ICommandResult<Attachment>> ValidateAsync(Attachment attachment)
        {

            // We need an attachment to validate
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            // Our result
            var result = new CommandResult<Attachment>();

            // Our attachment must have a name
            if (string.IsNullOrEmpty(attachment.Name))
            {
                return result.Failed("The attachment must have a name");
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to post attachments
            if (user == null)
            {
                return result.Failed("You must be authenticated to post attachments");
            }

            // Get users attachment options (max file size, total available space & allowed extensions)
            var options = await _attachmentOptionsFactory.GetOptionsAsync(user);

            // We need options to validate
            if (options == null)
            {
                return result.Failed("Could not obtain attachment settings for your account..");
            }

            // Compile errors
            var errors = new List<string>();

            // Validate file size 

            // Is the file larger than our max file size?
            if (attachment.ContentLength > options.MaxFileSize)
            {
                var error = T["The file is {0} which exceeds your configured maximum allowed file size of {1}."];
                errors.Add(string.Format(
                            error.Value,
                            result.Response.ContentLength.ToFriendlyFileSize(),
                            options.MaxFileSize.ToFriendlyFileSize()));
            }

            // Validate file extension

            var validExtension = false;   
            if (!string.IsNullOrEmpty(attachment.Extension))
            {
                foreach (var allowedExtension in options.AllowedExtensions)
                {
                    if (attachment.Extension.Equals($".{allowedExtension}", StringComparison.OrdinalIgnoreCase))
                    {
                        validExtension = true;
                    }
                }
            }
            if (!validExtension)
            {
                var allowedExtensions = string.Join(",", options.AllowedExtensions.Select(e => e));
                if (!string.IsNullOrEmpty(allowedExtensions))
                {
                    // Our extension does not appear within the allowed extensions white list
                    var error = T["The file is not an allowed type. You are allowed to attach the following types:- {0}"];
                    errors.Add(string.Format(
                                error.Value,
                                allowedExtensions.Replace(",", ", ")));
                }
                else
                {
                    // We don't have any configured allowed extensions
                    var error = T["The file is not an allowed type. No allowed file extensions have been configured for your account."];
                    errors.Add(error.Value);
                }
            }

            // Validate available space 

            var validSpace = false;
            long currentLength = 0;
            var info = await _attachmentInfoStore.GetByUserIdAsync(user?.Id ?? 0);
            if (info != null)
            {
                // Ensure the upload would not exceed available space    
                if ((info.Length + attachment.ContentLength) <= options.AvailableSpace)
                {
                    validSpace = true;
                }
                currentLength = info.Length;
            }

            if (!validSpace)
            {
                var remaining = options.AvailableSpace - currentLength;
                if (remaining < 0) remaining = 0;
                var error = T["Not enough free space. You have {0} of free space available but the file was {1}."];
                errors.Add(string.Format(
                            error.Value,
                            remaining.ToFriendlyFileSize(),
                            attachment.ContentLength.ToFriendlyFileSize()));
            }

            return errors.Count > 0
                ? result.Failed(errors)
                : result.Success(attachment);

        }

    }

}
