using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;

namespace Plato.Users.Notifications.ViewModels
{

    public class EditNotificationsViewModel
    {

        public int Id { get; set; }

        public IDictionary<string, IEnumerable<INotificationType>> CategorizedNotificationTypes { get; set; }

        public IEnumerable<UserNotificationType> EnabledNotificationTypes { get; set; }
        
    }

}
