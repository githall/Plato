using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Models.Features;
using PlatoCore.Models.Shell;

namespace PlatoCore.Features.Abstractions
{
    public interface IShellDescriptorManager
    {

        Task<IShellDescriptor> GetEnabledDescriptorAsync();

        Task<IEnumerable<IShellFeature>> GetEnabledFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeaturesAsync(string[] featureIds);
            
        Task<IShellFeature> GetFeatureAsync(string featureId);

        Task<IEnumerable<IShellFeature>> GetFeatureDependenciesAsync(string featureId);

        Task<IEnumerable<IShellFeature>> GetDependentFeaturesAsync(string featureId);

    }

}
