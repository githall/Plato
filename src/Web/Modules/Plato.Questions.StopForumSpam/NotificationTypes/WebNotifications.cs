using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.StopForumSpam.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification QuestionSpam =
            new WebNotification("QuestionSpamWeb", "Spam Issues",
                "Show me a web notification for each question detected as SPAM.");

        public static readonly WebNotification AnswerSpam =
            new WebNotification("AnswerSpamWeb", "Spam Issue Comments",
                "Show me a web notification for each question answer detected as SPAM.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
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
                        QuestionSpam,
                        AnswerSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
                    }
                }
            };
        }

    }

}
