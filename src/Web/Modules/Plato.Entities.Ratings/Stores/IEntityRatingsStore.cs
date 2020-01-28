using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Entities.Ratings.Stores
{
    public interface IEntityRatingsStore<TModel> : IStore<TModel> where TModel : class
    {
     
        Task<IEnumerable<TModel>> SelectEntityRatingsByEntityId(int entityId);

        Task<IEnumerable<TModel>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId);

    }

}
