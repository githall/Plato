using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Stores.Abstractions.Shell
{

    public interface IShellFeatureStore<T> : IStore<T> where T : class
    {
        Task<IEnumerable<T>> SelectFeatures();
    }
    
}
