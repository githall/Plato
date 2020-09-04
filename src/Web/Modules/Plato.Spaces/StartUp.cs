using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.SetUp;
using Plato.Spaces.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Assets.Abstractions;
using Plato.Spaces.Assets;

namespace Plato.Spaces
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

            // Set-up event handler
            //services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Feature installation event handler
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            // Index
            routes.MapAreaRoute(
                name: "Spaces",
                areaName: "Plato.Spaces",
                template: "spaces",
                defaults: new { controller = "Home", action = "Index" }
            );

        }

    }

}