using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;

namespace PlatoCore.Notifications
{
    public class DummyUserNotificationTypeDefaults : IUserNotificationTypeDefaults
    {

        public IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUser user)
        {
            return new List<UserNotificationType>();
        }
    }

}
