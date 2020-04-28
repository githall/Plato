﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Docs.NotificationTypes;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Entities.Stores;

namespace Plato.Docs.Notifications
{
    public class CommentReportWeb : INotificationProvider<ReportSubmission<DocComment>>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _urlHelper;
        private readonly IEntityStore<Doc> _entityStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public CommentReportWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ICapturedRouterUrlHelper urlHelper,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            IEntityStore<Doc> entityStore)
        {
            _urlHelper = urlHelper;
            _userNotificationManager = userNotificationManager;
            _entityStore = entityStore;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<ICommandResult<ReportSubmission<DocComment>>> SendAsync(INotificationContext<ReportSubmission<DocComment>> context)
        {
            // Validate
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Notification == null)
            {
                throw new ArgumentNullException(nameof(context.Notification));
            }

            if (context.Notification.Type == null)
            {
                throw new ArgumentNullException(nameof(context.Notification.Type));
            }

            if (context.Notification.To == null)
            {
                throw new ArgumentNullException(nameof(context.Notification.To));
            }

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.CommentReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(context.Model.What.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return null;
            }

            // Create result
            var result = new CommandResult<ReportSubmission<DocComment>>();
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = context.Model.What.Id
            });

            // Get reason given text
            var reasonText = S["Comment Reported"];
            if (ReportReasons.Reasons.ContainsKey(context.Model.Why))
            {
                reasonText = S[ReportReasons.Reasons[context.Model.Why]];
            }

            // Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = reasonText.Value,
                Message = S["A comment has been reported!"],
                Url = url,
                CreatedUserId = context.Notification.From?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Create notification
            var userNotificationResult = await _userNotificationManager.CreateAsync(userNotification);
            if (userNotificationResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(userNotificationResult.Errors?.ToArray());
            
        }

    }

}
