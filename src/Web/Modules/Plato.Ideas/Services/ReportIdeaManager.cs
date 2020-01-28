using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using PlatoCore.Models.Users;
using Plato.Entities.Models;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using Plato.Ideas.NotificationTypes;
using PlatoCore.Tasks.Abstractions;

namespace Plato.Ideas.Services
{
    
    public class ReportIdeaManager : IReportEntityManager<Idea> 
    {

        private readonly INotificationManager<ReportSubmission<Idea>> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDeferredTaskManager _deferredTaskManager;

        public ReportIdeaManager(
            INotificationManager<ReportSubmission<Idea>> notificationManager,
            IPlatoUserStore<User> platoUserStore,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDeferredTaskManager deferredTaskManager)
        {
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
        }

        public Task ReportAsync(ReportSubmission<Idea> submission)
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

                var from = submission.Who ?? await _platoUserStore.GetPlatoBotAsync();

                // Send notifications
                foreach (var user in users.Data)
                {

                    // Web notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.IdeaReport))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.IdeaReport)
                        {
                            To = user,
                            From = from
                        }, submission);
                    }

                    // Email notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.IdeaReport))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.IdeaReport)
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
