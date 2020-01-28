using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Notifications.Abstractions;

namespace PlatoCore.Notifications
{
    public class DummyUserNotificationsManager : IUserNotificationsManager<UserNotification>
    {

        // This implementation is overriden when Plato.Notificaitons is enabled

        public Task<ICommandResult<UserNotification>> CreateAsync(UserNotification model)
        {
            return Task.FromResult((ICommandResult<UserNotification>)new CommandResult<UserNotification>());
        }

        public Task<ICommandResult<UserNotification>> UpdateAsync(UserNotification model)
        {
            return Task.FromResult((ICommandResult<UserNotification>)new CommandResult<UserNotification>());
        }

        public Task<ICommandResult<UserNotification>> DeleteAsync(UserNotification model)
        {
            return Task.FromResult((ICommandResult<UserNotification>)new CommandResult<UserNotification>());
        }
    }
}
