using System.Collections.Generic;
using PlatoCore.Models.Notifications;

namespace PlatoCore.Notifications.Abstractions
{
 
    public class DefaultNotificationTypes
    {

        public string RoleName { get; set; }

        public IEnumerable<INotificationType> NotificationTypes { get; set; }

    }
    
}
