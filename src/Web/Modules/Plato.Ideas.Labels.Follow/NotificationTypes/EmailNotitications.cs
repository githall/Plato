using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.Labels.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewLabel =
            new EmailNotification(
                "NewIdeaLabelEmail",
                "Idea Labels",
                "Send me an email notification for each new idea associated with a label I'm following.");

        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {

            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewLabel
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
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                }
            };

        }

    }

}
