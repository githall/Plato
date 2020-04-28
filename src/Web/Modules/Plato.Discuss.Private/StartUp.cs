using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using Plato.Discuss.Private.Handlers;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using Plato.Discuss.Private.Navigation;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Discuss.Private.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Private
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
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, PostMenu>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}