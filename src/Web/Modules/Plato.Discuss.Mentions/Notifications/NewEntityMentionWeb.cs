﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Discuss.Mentions.NotificationTypes;
using Plato.Entities.Extensions;

namespace Plato.Discuss.Mentions.Notifications
{

    public class NewEntityMentionWeb : INotificationProvider<Topic>
    {
                
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public NewEntityMentionWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            IContextFacade contextFacade)
        {            
            _userNotificationManager = userNotificationManager;
            _contextFacade = contextFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Topic>> SendAsync(INotificationContext<Topic> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewMention.Name, StringComparison.Ordinal))
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
            var result = new CommandResult<Topic>();
            
            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S["New Mention"].Value,
                Message = S["You've been mentioned by "].Value + context.Model.CreatedBy.DisplayName,
                CreatedUserId = context.Model.CreatedUserId,
                Url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = context.Model.Id,
                    ["opts.alias"] = context.Model.Alias
                })
            };

            var userNotificationResult = await _userNotificationManager.CreateAsync(userNotification);
            if (userNotificationResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(userNotificationResult.Errors?.ToArray());
            
        }

    }
    
}
