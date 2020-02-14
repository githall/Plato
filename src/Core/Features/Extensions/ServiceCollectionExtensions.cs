using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Features.Abstractions;

namespace PlatoCore.Features.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoShellFeatures(
            this IServiceCollection services)
        {
            
            services.TryAddScoped<IFeatureEventManager, FeatureEventManager>();
            services.TryAddScoped<IFeatureFacade, FeatureFacade>();

            return services;

        }

    }

}
