using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.Models;
using Plato.Ideas.Private.Handlers;
using Plato.Ideas.Private.Navigation;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Ideas.Private.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.Private
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, PostMenu>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}