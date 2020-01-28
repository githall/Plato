using System.Collections.Generic;

namespace PlatoCore.Models.Notifications
{
    
    public interface IUserNotificationTypes
    {
        IEnumerable<UserNotificationType> NotificationTypes { get; set; }
    }
    
}
