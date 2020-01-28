using System.Collections.Generic;

namespace PlatoCore.Notifications.Abstractions
{

    public interface INotificationTypeProvider
    {
    
        IEnumerable<DefaultNotificationTypes> GetNotificationTypes();
        
        IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes();

    }

}
