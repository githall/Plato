using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Services;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;

namespace Plato.Discuss.Services
{

    public class ReportReplyManager : IReportEntityManager<Reply>
    {

        private readonly INotificationManager<ReportSubmission<Reply>> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDeferredTaskManager _deferredTaskManager;

        public ReportReplyManager(
            INotificationManager<ReportSubmission<Reply>> notificationManager,
            IPlatoUserStore<User> platoUserStore,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDeferredTaskManager deferredTaskManager)
        {
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
        }
        
        public Task ReportAsync(ReportSubmission<Reply> submission)
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
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.ReplyReport))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.ReplyReport)
                        {
                            To = user,
                            From = from
                        }, submission);
                    }

                    // Email notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.ReplyReport))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.ReplyReport)
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
