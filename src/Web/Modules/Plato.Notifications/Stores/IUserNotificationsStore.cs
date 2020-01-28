using System;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Notifications.Stores
{

    public interface IUserNotificationsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<bool> UpdateReadDateAsync(int userId, DateTimeOffset? readDate);

    }

}
