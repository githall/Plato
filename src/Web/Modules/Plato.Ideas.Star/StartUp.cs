using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using Plato.Ideas.Star.Subscribers;
using Plato.Ideas.Star.ViewProviders;
using Plato.Ideas.Models;
using Plato.Ideas.Star.Handlers;
using Plato.Ideas.Star.QueryAdapters;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Ideas.Star
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // View providers
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();

            // Star subscribers
            services.AddScoped<IBrokerSubscriber, StarSubscriber>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Idea>, IdeaQueryAdapter>();
            
        }

    }

}