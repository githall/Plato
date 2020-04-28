﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Users;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Discuss.Models;
using Plato.Discuss.StopForumSpam.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Http.Abstractions;

namespace Plato.Discuss.StopForumSpam.SpamOperators
{

    public class TopicOperator : ISpamOperatorProvider<Topic>
    {
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<Topic> _notificationManager;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IClientIpAddress _clientIpAddress;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly ISpamChecker _spamChecker;

        public TopicOperator(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<Topic> notificationManager,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IClientIpAddress clientIpAddress,
            IEntityStore<Topic> topicStore,
            ISpamChecker spamChecker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _clientIpAddress = clientIpAddress;
            _platoUserStore = platoUserStore;
            _spamChecker = spamChecker;
            _topicStore = topicStore;
        }

        public async Task<ISpamOperatorResult<Topic>> ValidateModelAsync(ISpamOperatorContext<Topic> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Topic.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get user for entity
            var user = await BuildUserAsync(context.Model);
            if (user == null)
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<Topic>();
            
            // Check if user is already flagged as SPAM within Plato
            if (user.IsSpam)
            {
                return result.Failed(context.Model, context.Operation);
            }

            // Check StopForumSpam service
            var spamResult = await _spamChecker.CheckAsync(user);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);
            
        }

        public async Task<ISpamOperatorResult<Topic>> UpdateModelAsync(ISpamOperatorContext<Topic> context)
        {

            // Perform validation
            var validation = await ValidateModelAsync(context);

            // Create result
            var result = new SpamOperatorResult<Topic>();
            
            // Not an operator of interest
            if (validation == null)
            {
                return result.Success(context.Model);
            }

            // If validation succeeded no need to perform further actions
            if (validation.Succeeded)
            {
                return result.Success(context.Model);
            }
            
            // Get entity author
            var user = await BuildUserAsync(context.Model);
            if (user == null)
            {
                return null;
            }
            
            // Flag user as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                var bot = await _platoUserStore.GetPlatoBotAsync();

                // Mark user as SPAM
                if (!user.IsSpam)
                {
                    user.IsSpam = true;
                    user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                    user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                    await _platoUserStore.UpdateAsync(user);
                }

                // Mark entity as SPAM
                if (!context.Model.IsSpam)
                {
                    context.Model.IsSpam = true;
                    await _topicStore.UpdateAsync(context.Model);
                }

            }

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {
                await NotifyAsync(context);
            });

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        async Task NotifyAsync(ISpamOperatorContext<Topic> context)
        {

            // Get users to notify
            var users = await GetUsersAsync(context.Operation);

            // No users to notify
            if (users == null)
            {
                return;
            }

            // Get bot
            var bot = await _platoUserStore.GetPlatoBotAsync();

            // Send notifications
            foreach (var user in users)
            {

                // Web notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.TopicSpam))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.TopicSpam)
                    {
                        To = user,
                        From = bot
                    }, context.Model);
                }

                // Email notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.TopicSpam))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.TopicSpam)
                    {
                        To = user
                    }, context.Model);
                }

            }

        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            var roleNames = new List<string>(2);
            if (operation.NotifyAdmin)
                roleNames.Add(DefaultRoles.Administrator);
            if (operation.NotifyStaff)
                roleNames.Add(DefaultRoles.Staff);
            if (roleNames.Count == 0)
                return null;
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.RoleName.IsIn(roleNames.ToArray());
                })
                .ToList();
            return users?.Data;

        }

        async Task<User> BuildUserAsync(IEntity entity)
        {

            var user = await _platoUserStore.GetByIdAsync(entity.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Ensure we check against the IP address being used at the time of the post
            user.IpV4Address = entity.IpV4Address;
            user.IpV6Address = entity.IpV6Address;
            return user;

        }


    }

}


