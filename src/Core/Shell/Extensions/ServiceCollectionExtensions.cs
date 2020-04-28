using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Cache.Extensions;
using PlatoCore.Features.Extensions;
using PlatoCore.FileSystem;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Hosting.Web;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Messaging.Extensions;
using PlatoCore.Navigation.Extensions;
using PlatoCore.Http.Extensions;
using PlatoCore.Repositories.Extensions;
using PlatoCore.Reputations.Extensions;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Stores.Extensions;
using PlatoCore.Tasks.Extensions;

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

        /// <summary>
        /// Core host services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlatoShell(
            this IServiceCollection services)
        {

            // ----------------
            // Core module management
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

        /// <summary>
        /// Services specific to each tenant.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlatoTenant(
            this IServiceCollection services)
        {

            // HTTP
            services.AddShellHttp();

            // Broker
            services.AddShellMessaging();

            // Reputation
            services.AddShellReputations();

            // Tasks
            services.AddShellTasks();

            // Caching
            services.AddShellCaching();

            // Navigation
            services.AddShellNavigation();

            // Data access
            services.AddShellRepositories();
            services.AddShellStores();

            // Shell features
            services.AddShellFeatures();

            // Facades
            services.AddScoped<IContextFacade, ContextFacade>();

            // The captured router is used to resolve URLs for background or deferred tasks 
            // Background and deferred tasks don't have access to the current HttpContext
            // Our captured router must be a singleton so the initial configuration performed
            // by the PlatoRouterMiddleware is persisted throughout the application life cycle
            services.AddSingleton<ICapturedRouter, CapturedRouter>();
            services.AddScoped<ICapturedRouterUrlHelper, CapturedRouterUrlHelper>();
            services.AddSingleton<ICapturedHttpContext, CapturedHttpContext>();

            return services;

        }

    }

}
