using Microsoft.Extensions.DependencyInjection;
using PlatoCore.FileSystem;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Shell.Abstractions;

namespace PlatoCore.Shell.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureShell(
            this IServiceCollection services,
            string shellLocation)
        {
            return services.Configure<ShellOptions>(options =>
            {
                options.Location = shellLocation;
            });
        }

        public static IServiceCollection AddPlatoShell(
            this IServiceCollection services)
        {

            // ----------------
            // core module management
            // ----------------

            services.AddSingleton<IAppDataFolder, PhysicalAppDataFolder>();
            services.AddSingleton<ISitesFolder, PhysicalSitesFolder>();

            // shell / tenant context

            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();
            services.AddSingleton<IShellContextFactory, ShellContextFactory>();
            {
                services.AddSingleton<ICompositionStrategy, CompositionStrategy>();
                services.AddSingleton<IShellContainerFactory, ShellContainerFactory>();
            }

            services.AddSingleton<IRunningShellTable, RunningShellTable>();
            
            return services;
        }
        
    }

}
