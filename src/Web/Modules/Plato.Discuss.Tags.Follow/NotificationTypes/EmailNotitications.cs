using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Tags.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewTag =
            new EmailNotification("NewTopicTagEmail",
                "Topic Tags",
                "Send me an email notification for each new topic associated with a tag I'm following.");

        public static readonly EmailNotification NewReplyTag =
            new EmailNotification("NewTopicReplyTagEmail",
                "Topic Reply Tags",
                "Send me an email notification for each new reply associated with a tag I'm following.");

        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        NewTag,
                        NewReplyTag
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewTag,
                        NewReplyTag
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewTag,
                        NewReplyTag
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
                        NewTag,
                        NewReplyTag
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewTag,
                        NewReplyTag
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewTag,
                        NewReplyTag
                    }
                }
            };

        }

    }

}
