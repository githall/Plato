using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;

namespace PlatoCore.Features.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddShellFeatures(
            this IServiceCollection services)
        {

            services.AddTransient<IShellDescriptorManager, ShellDescriptorManager>();
            services.AddTransient<IShellFeatureManager, ShellFeatureManager>();
            services.AddSingleton<IFeatureEventManager, FeatureEventManager>();
            services.AddScoped<IFeatureFacade, FeatureFacade>();

            return services;

        }

    }

}
