using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Files.Sharing.ViewProviders;
using PlatoCore.Layout.ViewProviders;
using Plato.Files.Models;
using Plato.Files.Sharing.Handlers;
using Plato.Files.Sharing.Assets;
using PlatoCore.Assets.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

namespace Plato.Files.Sharing
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

            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // View providers     
            services.AddScoped<IViewProviderManager<File>, ViewProviderManager<File>>();
            services.AddScoped<IViewProvider<File>, AdminViewProvider>();
       
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "ShareFile",
                areaName: "Plato.Files.Sharing",
                template: "admin/files/share/{id:int}",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}