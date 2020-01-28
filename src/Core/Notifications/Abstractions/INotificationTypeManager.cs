using System.Collections.Generic;
using PlatoCore.Models.Notifications;

namespace PlatoCore.Notifications.Abstractions
{

    public interface INotificationTypeManager
    {
        
        IEnumerable<INotificationType> GetNotificationTypes(IEnumerable<string> roleNames);
        
        IEnumerable<INotificationType> GetDefaultNotificationTypes(IEnumerable<string> roleNames);

        IDictionary<string, IEnumerable<INotificationType>> GetCategorizedNotificationTypes(IEnumerable<string> roleNames);

    }
    
}
