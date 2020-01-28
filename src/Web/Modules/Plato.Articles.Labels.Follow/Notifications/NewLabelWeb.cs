using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Labels.Follow.NotificationTypes;
using Plato.Articles.Models;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;

namespace Plato.Articles.Labels.Follow.Notifications
{

    public class NewLabelWeb : INotificationProvider<Article>
    {

        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public NewLabelWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper)
        {

            _userNotificationManager = userNotificationManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Article>> SendAsync(INotificationContext<Article> context)
        {

            // Create result
            var result = new CommandResult<Article>();

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.NewLabel.Name, StringComparison.Ordinal))
            {
                return result.Failed($"Skipping notification '{WebNotifications.NewLabel.Name}' as this does not match '{context.Notification.Type.Name}'.");
            }

            // Build user notification
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();

            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = context.Model.Title,
                Message = S["An article has been posted with a label your following"],
                CreatedUserId = context.Model.CreatedUserId,
                Url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Articles",
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
