﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Badges.NotificationTypes;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Users.Badges.Notifications
{

    public class NewBadgeWeb : INotificationProvider<Badge>
    {
        
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _urlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public NewBadgeWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper urlHelper)
        {
            _userNotificationManager = userNotificationManager;
            _urlHelper = urlHelper;
            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<ICommandResult<Badge>> SendAsync(INotificationContext<Badge> context)
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewBadge.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Badge>();
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Users.Badges",
                ["controller"] = "Profile",
                ["action"] = "Index",
                ["opts.id"] = context.Notification.To.Id,
                ["opts.alias"] = context.Notification.To.Alias
            });

            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = context.Model.Title,
                Message = $"{S["You've earned the"].Value} '{context.Model.Title}' {S["badge"].Value}",
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
