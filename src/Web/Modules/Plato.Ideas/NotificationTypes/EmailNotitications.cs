using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification IdeaReport =
            new EmailNotification("IdeaReportEmail", "Idea Reports",
                "Send me an email notification each time an idea is reported.");

        public static readonly EmailNotification IdeaCommentReport =
            new EmailNotification("IdeaCommentReportEmail", "Idea Comment Reports",
                "Send me an email notification each time a comment to an idea is reported.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IdeaReport,
                        IdeaCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IdeaReport,
                        IdeaCommentReport
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
                        IdeaReport,
                        IdeaCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IdeaReport,
                        IdeaCommentReport
                    }
                }

            };
        }

    }

}
