﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Plato.Articles.Models;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Articles.Mentions.NotificationTypes;
using Plato.Entities.Extensions;
using PlatoCore.Models.Users;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Mentions.Notifications
{

    public class NewEntityMentionEmail : INotificationProvider<Article>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;

        public NewEntityMentionEmail(
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
        }

        public async Task<ICommandResult<Article>> SendAsync(INotificationContext<Article> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewMention.Name, StringComparison.Ordinal))
            {
                return null;
            }
            
            // We always need a model
            if (context.Model == null)
            {
                return null;
            }

            // The entity should be visible
            if (context.Model.IsHidden())
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<Article>();

            // Get email template
            const string templateId = "NewArticlesMention";

            // Tasks run in a background thread and don't have access to HttpContext
            // Create a dummy principal to represent the user so we can still obtain
            // the current culture for the email
            var principal = await _claimsPrincipalFactory.CreateAsync((User)context.Notification.To);
            var culture = await _contextFacade.GetCurrentCultureAsync(principal.Identity);
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build topic url
                var baseUri = await _contextFacade.GetBaseUrlAsync();
                var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Articles",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = context.Model.Id,
                    ["opts.alias"] = context.Model.Alias
                });
                
                // Build message from template
                var message = email.BuildMailMessage();
                message.Body = string.Format(
                    email.Message,
                    context.Notification.To.DisplayName,
                    context.Model.Title,
                    baseUri + url);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(context.Notification.To.Email));

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(context.Model);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

    }
    
}
