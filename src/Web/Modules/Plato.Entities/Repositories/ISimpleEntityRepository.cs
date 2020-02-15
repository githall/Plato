using PlatoCore.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Entities.Repositories
{
    public interface ISimpleEntityRepository<T> : IQueryableRepository<T> where T : class
    {

        Task<T> SelectByIdAsync(int id);

        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);
    }

}
