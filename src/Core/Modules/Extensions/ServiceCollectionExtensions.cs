using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Modules.Configuration;
using PlatoCore.Modules.Loader;
using PlatoCore.Modules.Locator;
using PlatoCore.Modules.Models;

namespace PlatoCore.Modules.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoModules(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<ITypedModuleProvider, TypedModuleProvider>();

            return services;

        }

    }

}
