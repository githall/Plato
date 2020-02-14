using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Features.Abstractions
{
    public interface IShellFeatureManager
    {

        Task<IEnumerable<IFeatureEventContext>> EnableFeatureAsync(string featureId);

        Task<IEnumerable<IFeatureEventContext>> EnableFeaturesAsync(string[] featureIds);

        Task<IEnumerable<IFeatureEventContext>> DisableFeatureAsync(string featureId);

        Task<IEnumerable<IFeatureEventContext>> DisableFeaturesAsync(string[] featureIds);

    }

}
