using System.Collections.Generic;
using PlatoCore.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;

namespace Plato.Notifications.Models
{
 
    public class UserNotificationTypes : Serializable, IUserNotificationTypes
    {

        public IEnumerable<UserNotificationType> NotificationTypes { get; set; }

    }

}
