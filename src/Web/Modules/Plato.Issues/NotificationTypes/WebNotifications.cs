using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Issues.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification IssueReport =
            new WebNotification("IssueReportWeb", "Issue Reports",
                "Show me a web notification each time an issue is reported.");

        public static readonly WebNotification IssueCommentReport =
            new WebNotification("IssueCommentReportWeb", "Issue Comment Reports",
                "Show me a web notification each time an issue comment is reported.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                }

            };
        }

        public IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                }
            };
        }

    }

}
