using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;

namespace Plato.Discuss.Follow.Notifications
{

    public class NewReplyWeb : INotificationProvider<Reply>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Topic> _topicStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public NewReplyWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Topic> topicStore)
        {
         
            _userNotificationManager = userNotificationManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _topicStore = topicStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Reply>> SendAsync(INotificationContext<Reply> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewReply.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Reply>();

            // Get the topic for the reply
            var topic = await _topicStore.GetByIdAsync(context.Model.EntityId);
            if (topic == null)
            {
                return result.Failed($"No entity could be found with the Id of {context.Model.EntityId} when sending the topic follow notification '{WebNotifications.NewReply.Name}'.");
            }

            // Get base Uri
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();

            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = topic.Title,
                Message = S["A reply has been posted within a topic your following"],
                CreatedUserId = context.Model.CreatedUserId,
                Url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = topic.Id,
                    ["opts.alias"] = topic.Alias,
                    ["opts.replyId"] = context.Model.Id
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
