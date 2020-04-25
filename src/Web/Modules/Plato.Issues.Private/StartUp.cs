using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using Plato.Issues.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using Plato.Issues.Private.Navigation;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Security.Abstractions;
using Plato.Issues.Private.Handlers;
using Plato.Issues.Private.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.Private
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
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, PostMenu>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}