using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Labels.Stores
{

    public interface ILabelStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);
    }
    
}
