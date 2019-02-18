﻿using System.Collections.Generic;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationTypeProvider
    {
        IEnumerable<INotificationType> GetNotificationTypes();

        IEnumerable<INotificationType> GetDefaultNotificationTypes();

        IEnumerable<DefaultNotificationTypes> GetNotificationTypes2();
        
        IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes2();

    }

}
