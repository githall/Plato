using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;

namespace PlatoCore.Notifications.Abstractions
{
    public interface IUserNotificationTypeDefaults
    {
        IEnumerable<UserNotificationType> GetUserNotificationTypesWithDefaults(IUser user);

    }
}
