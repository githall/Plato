using PlatoCore.Abstractions;

namespace PlatoCore.Notifications.Abstractions
{
    public interface IUserNotificationsManager<TNotification> : ICommandManager<TNotification> where TNotification : class
    {

    }
}
