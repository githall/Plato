using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification IdeaSpam =
            new EmailNotification("IdeaSpamEmail", "Idea Spam",
                "Send me an email notification for each idea detected as SPAM.");

        public static readonly EmailNotification CommentSpam =
            new EmailNotification("IdeaCommentSpamEmail", "Idea Comment Spam",
                "Send me an email notification for each idea comment detected as SPAM.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IdeaSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IdeaSpam,
                        CommentSpam
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
                        IdeaSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IdeaSpam,
                        CommentSpam
                    }
                }

            };
        }

    }

}
