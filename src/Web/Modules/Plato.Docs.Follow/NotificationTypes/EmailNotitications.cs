using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Docs.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification EntityReply =
            new EmailNotification("DocReplyEmail", "Doc Comments", "Send me an email notification for each new comment within articles I'm following.");

        public static readonly EmailNotification EntityUpdate =
            new EmailNotification("DocUpdateEmail", "Doc Updates", "Send me an email notification for changes within articles I'm following.");

        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        EntityReply,
                        EntityUpdate
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        EntityReply,
                        EntityUpdate
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        EntityReply,
                        EntityUpdate
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
                        EntityReply,
                        EntityUpdate
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        EntityReply,
                        EntityUpdate
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        EntityReply,
                        EntityUpdate
                    }
                }

            };

        }

    }

}
