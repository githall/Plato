using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Models.Notifications;

namespace PlatoCore.Notifications.Abstractions
{
    public interface INotificationProvider<TModel> where TModel : class
    {
        Task<ICommandResult<TModel>> SendAsync(INotificationContext<TModel> context);
    }
}
