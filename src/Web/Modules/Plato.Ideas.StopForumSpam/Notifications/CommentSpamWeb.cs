﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using PlatoCore.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Ideas.StopForumSpam.NotificationTypes;
using Plato.Entities.Stores;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Ideas.StopForumSpam.Notifications
{
    public class CommentSpamWeb : INotificationProvider<IdeaComment>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly IEntityStore<Idea> _entityStore;
        private readonly ICapturedRouterUrlHelper _urlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public CommentSpamWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper urlHelper,
            IEntityStore<Idea> entityStore)
        {

            _userNotificationManager = userNotificationManager;
            _entityStore = entityStore;
            _urlHelper = urlHelper;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<ICommandResult<IdeaComment>> SendAsync(INotificationContext<IdeaComment> context)
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
            if (!context.Notification.Type.Name.Equals(WebNotifications.CommentSpam.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<IdeaComment>();

            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(context.Model.EntityId);

            // Ensure we found the entity
            if (entity == null)
            {
                return result.Failed(
                    $"No entity with id '{context.Model.EntityId}' exists. Failed to send reply spam web notification.");
            }
            
            var baseUri = await _urlHelper.GetBaseUrlAsync();
            var url = _urlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Ideas",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = context.Model.Id
            });

            //// Build notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = S["Possible SPAM"].Value,
                Message = S["A doc comment has been detected as SPAM!"],
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
