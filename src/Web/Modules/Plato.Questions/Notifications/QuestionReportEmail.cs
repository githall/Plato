﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Questions.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;
using PlatoCore.Models.Users;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Questions.Notifications
{
    public class QuestionReportEmail : INotificationProvider<ReportSubmission<Question>>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public QuestionReportEmail(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
            
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<ReportSubmission<Question>>> SendAsync(INotificationContext<ReportSubmission<Question>> context)
        {
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.QuestionReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<ReportSubmission<Question>>();

            // Get email template
            const string templateId = "NewQuestionReport";

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

            // Get reason given text
            var reasonText = S["No reason supplied"];
            if (ReportReasons.Reasons.ContainsKey(context.Model.Why))
            {
                reasonText = S[ReportReasons.Reasons[context.Model.Why]];
            }
            
            // Build topic url
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Questions",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = context.Model.What.Id,
                ["opts.alias"] = context.Model.What.Alias
            });

            // Build message from template
            var message = email.BuildMailMessage();
            message.Body = string.Format(
                email.Message,
                context.Notification.To.DisplayName,
                context.Model.What.Title,
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
