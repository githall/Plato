using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Follow.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {
        
        public static readonly WebNotification EntityReply =
            new WebNotification("ArticleReplyWeb", "Article Comments", "Show me a web notification for each new comment within articles I'm following.");

        public static readonly WebNotification EntityUpdate =
            new WebNotification("ArticleUpdateWeb", "Article Updates", "Show me a web notification for updates within articles I'm following.");

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
