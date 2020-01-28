using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Tags.Follow.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewTag =
            new WebNotification("NewTopicTagWeb",
                "Topic Tags",
                "Show me a web notification for each new topic associated with a tag I'm following.");

        public static readonly WebNotification NewReplyTag =
            new WebNotification("NewTopicReplyTagWeb",
                "Topic Reply Tags",
                "Show me a web notification for each new reply associated with a tag I'm following.");

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
