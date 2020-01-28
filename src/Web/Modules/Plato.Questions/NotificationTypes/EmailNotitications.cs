using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification QuestionReport =
            new EmailNotification("QuestionReportEmail", "Question Reports",
                "Send me an email notification each time a question is reported.");

        public static readonly EmailNotification AnswerReport =
            new EmailNotification("AnswerReportEmail", "Question Answer Reports",
                "Send me an email notification each time a questions answer is reported.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
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
                        QuestionReport,
                        AnswerReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionReport,
                        AnswerReport
                    }
                }

            };
        }

    }

}
