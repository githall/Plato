using System.Threading.Tasks;
using PlatoCore.Models.Users;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;
using Plato.Discuss.NotificationTypes;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.Models;

namespace Plato.Discuss.Services
{

    public class ReportTopicManager : IReportEntityManager<Topic> 
    {

        private readonly INotificationManager<ReportSubmission<Topic>> _notificationManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public ReportTopicManager(
            INotificationManager<ReportSubmission<Topic>> notificationManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
        }

        public Task ReportAsync(ReportSubmission<Topic> submission)
        {

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {

                // Get users to notify
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.IsIn(new[]
                        {
                            DefaultRoles.Administrator,
                            DefaultRoles.Staff
                        });
                    })
                    .ToList();

                // No users to notify
                if (users?.Data == null)
                {
                    return;
                }

                // If anonymous use bot as sender
                var from = submission.Who ??
                           await _platoUserStore.GetPlatoBotAsync();

                // Send notifications
                foreach (var user in users.Data)
                {

                    // Web notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.TopicReport))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.TopicReport)
                        {
                            To = user,
                            From = from
                        }, submission);
                    }

                    // Email notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.TopicReport))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.TopicReport)
                        {
                            To = user
                        }, submission);
                    }

                }

            });

            return Task.CompletedTask;
            
        }

    }

}
