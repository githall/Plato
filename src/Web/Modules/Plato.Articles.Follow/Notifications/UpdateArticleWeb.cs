﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Follow.NotificationTypes;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;

namespace Plato.Articles.Follow.Notifications
{

    public class UpdatedArticleWeb : INotificationProvider<Article>
    {
        
        private readonly IUserNotificationsManager<UserNotification> _userNotificationManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Article> _entityStore;
     
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public UpdatedArticleWeb(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserNotificationsManager<UserNotification> userNotificationManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Article> entityStore)
        {
         
            _userNotificationManager = userNotificationManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _entityStore = entityStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<ICommandResult<Article>> SendAsync(INotificationContext<Article> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(WebNotifications.EntityUpdate.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Article>();
            
            // Get base Uri
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();

            // Build user notification
            var userNotification = new UserNotification()
            {
                NotificationName = context.Notification.Type.Name,
                UserId = context.Notification.To.Id,
                Title = context.Model.Title,
                Message = S["I've just updated an articles your following"],
                CreatedUserId = context.Model.ModifiedUserId,
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
