using PlatoCore.Stores.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Entities.Stores
{
    public interface ISimpleEntityStore<TModel> : IQueryableStore<TModel> where TModel : class
    {

        Task<TModel> GetByIdAsync(int id);

        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);

        Task<IEnumerable<TModel>> GetParentsByIdAsync(int entityId);

        Task<IEnumerable<TModel>> GetChildrenByIdAsync(int entityId);

    }

}
