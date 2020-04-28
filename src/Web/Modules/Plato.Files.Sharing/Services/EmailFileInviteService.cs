using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Extensions;
using Plato.Files.Models;
using Plato.Files.Sharing.Models;
using Plato.Files.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;

namespace Plato.Files.Sharing.Services
{

    public class EmailFileInviteService : IEmailFileInviteService
    {

        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;        
        private readonly IContextFacade _contextFacade;
        private readonly IFileStore<File> _fileStore;
        private readonly IEmailManager _emailManager;        
        private readonly ILocaleStore _localeStore;        

        public IHtmlLocalizer T { get; }

        public EmailFileInviteService(  
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IHtmlLocalizer htmlLocalizer,            
            IContextFacade contextFacade,
            IFileStore<File> fileStore,
            IEmailManager emailManager,            
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            _fileStore = fileStore;

            T = htmlLocalizer;
        }

        public async Task<ICommandResult<FileInvite>> SendAttachmentInviteAsync(FileInvite invite)
        {

            if (invite == null)
            {
                throw new ArgumentException(nameof(invite));
            }

            // Create result
            var result = new CommandResult<FileInvite>();

            if (invite.FileId <= 0)
            {
                return result.Failed(T["A file is required to share"].Value);
            }

            if (string.IsNullOrEmpty(invite.Email))
            {
                return result.Failed(T["An email address is required"].Value);
            }

            // Get file
            var file = await _fileStore.GetByIdAsync(invite.FileId);

            // Ensure we found the file
            if (file == null)
            {
                return result.Failed(T["The file could not be found"].Value);
            }

            // Get email template
            const string templateId = "ShareFileAttachment";
            var culture = await _contextFacade.GetCurrentCultureAsync();            
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build message
                var message = email.BuildMailMessage();
                message.Subject = string.Format(
                     email.Subject,
                     invite.CreatedBy.DisplayName);
                message.Body = string.Format(
                    email.Message,
                    invite.CreatedBy.DisplayName);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(invite.Email.Trim()));
                message.Attachments.Add(file.ToAttachment());

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(invite);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

        public async Task<ICommandResult<FileInvite>> SendLinkInviteAsync(FileInvite invite)
        {

            if (invite == null)
            {
                throw new ArgumentException(nameof(invite));
            }

            // Create result
            var result = new CommandResult<FileInvite>();

            if (invite.FileId <= 0)
            {
                return result.Failed(T["A file is required to share"].Value);
            }

            if (string.IsNullOrEmpty(invite.Email))
            {
                return result.Failed(T["An email address is required"].Value);
            }

            // Get file
            var file = await _fileStore.GetByIdAsync(invite.FileId);

            // Ensure we found the file
            if (file == null)
            {
                return result.Failed(T["The file could not be found"].Value);
            }

            // Get email template
            const string templateId = "ShareFileLink";

            // Configured culture
            var culture = await _contextFacade.GetCurrentCultureAsync();

            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Ensure email is safe for URL
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(invite.Email));

                // Build invite URL
                var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
                var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Files.Sharing",
                    ["controller"] = "Home",
                    ["action"] = "Index",
                    ["id"] = invite.Id,
                    ["token"] = token,
                    ["alias"] = file.Alias
                });

                // Build message from template
                var message = email.BuildMailMessage();
                message.Subject = string.Format(
                    email.Subject,
                    invite.CreatedBy.DisplayName);
                message.Body = string.Format(
                    email.Message,
                    invite.CreatedBy.DisplayName,
                    file.Name,
                    file.ContentLength.ToFriendlyFileSize(),
                    baseUri + url);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(invite.Email));

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(invite);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

    }

}
