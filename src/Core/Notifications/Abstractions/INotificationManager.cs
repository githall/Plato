using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Models.Notifications;

namespace PlatoCore.Notifications.Abstractions
{

    public interface INotificationManager<TModel> where TModel : class
    {

        Task<IEnumerable<ICommandResult<TModel>>> SendAsync(INotification notification, TModel model);

    }
    
}
