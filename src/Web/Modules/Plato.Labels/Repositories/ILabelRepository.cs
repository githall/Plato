using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Labels.Repositories
{

    public interface ILabelRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);

    }

}
