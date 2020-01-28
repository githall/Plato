using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Metrics.Stores
{
    public interface IMetricsStore<TModel> : IStore<TModel> where TModel : class
    {
  
    }

}
