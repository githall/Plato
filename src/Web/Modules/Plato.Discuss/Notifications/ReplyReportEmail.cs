﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Notifications
{

    public class ReplyReportEmail : INotificationProvider<ReportSubmission<Reply>>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public ReplyReportEmail(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Topic> topicStore,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            _topicStore = topicStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<ReportSubmission<Reply>>> SendAsync(INotificationContext<ReportSubmission<Reply>> context)
        {

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.ReplyReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<ReportSubmission<Reply>>();

            // Get email template
            const string templateId = "NewReplyReport";

            // Tasks run in a background thread and don't have access to HttpContext
            // Create a dummy principal to represent the user so we can still obtain
            // the current culture for the email
            var principal = await _claimsPrincipalFactory.CreateAsync((User)context.Notification.To);
            var culture = await _contextFacade.GetCurrentCultureAsync(principal.Identity);
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email == null)
            {
                return result.Failed(
                    $"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");
            }

            // Get topic for reply
            var topic = await _topicStore.GetByIdAsync(context.Model.What.EntityId);

            // We need an topic for the reply
            if (topic == null)
            {
                return result.Failed(
                    $"No entity with id '{context.Model.What.EntityId}' exists. Failed to send reply spam email notification.");
            }
        
            // Build topic url
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = context.Model.What.Id
            });

            // Reason given text
            var reasonText = S["None Provided"];
            if (ReportReasons.Reasons.ContainsKey(context.Model.Why))
            {
                reasonText = S[ReportReasons.Reasons[context.Model.Why]];
            }
            
            // Build message from template
            var message = email.BuildMailMessage();
            message.Body = string.Format(
                email.Message,
                context.Notification.To.DisplayName,
                topic.Title,
                reasonText.Value,
                context.Model.Who.DisplayName,
                context.Model.Who.UserName,
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

    }
}
