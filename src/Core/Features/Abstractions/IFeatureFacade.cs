using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Features;

namespace PlatoCore.Features.Abstractions
{
    public interface IFeatureFacade
    {
        
        Task<IShellFeature> GetFeatureByIdAsync(string moduleId);

        Task<IEnumerable<IShellFeature>> GetFeatureUpdatesAsync();
        
    }

}
