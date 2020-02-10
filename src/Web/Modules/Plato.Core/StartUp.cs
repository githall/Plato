using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Core.Assets;
using Plato.Core.Configuration;
using Plato.Core.Handlers;
using Plato.Core.Middleware;
using Plato.Core.Models;
using Plato.Core.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Localization.Abstractions.Models;

namespace Plato.Core
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
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register view providers
            services.AddScoped<IViewProviderManager<HomeIndex>, ViewProviderManager<HomeIndex>>();
            services.AddScoped<IViewProvider<HomeIndex>, HomeViewProvider>();

            // Configure site options
            services.AddSingleton<IConfigureOptions<SiteOptions>, SiteOptionsConfiguration>();
            
            // Configure current culture
            services.Configure<LocaleOptions>(options =>
            {
                var contextFacade = services.BuildServiceProvider().GetService<IContextFacade>();
                options.WatchForChanges = false;
                options.Culture = contextFacade.GetCurrentCultureAsync().Result;
            });
            
            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Register client options middleware 
            app.UseMiddleware<SettingsClientOptionsMiddleware>();

            // Routes

            // Defaults
            var homeRoute = new HomeRoute();
            var homeAlias = "";

            // Configure defaults from site options
            var siteOptions = serviceProvider.GetRequiredService<IOptions<SiteOptions>>();
            if (siteOptions != null)
            {
                if (siteOptions.Value?.HomeRoute != null)
                {
                    homeRoute = siteOptions.Value.HomeRoute;
                }
                if (!string.IsNullOrEmpty(siteOptions.Value?.HomeAlias))
                {
                    homeAlias = siteOptions.Value.HomeAlias;
                }
            }

            // Get home page route parts
            var area = homeRoute["area"].ToString();
            var controller = homeRoute["controller"].ToString();
            var action = homeRoute["action"].ToString();

            // Homepage
            routes.MapAreaRoute(
                name: "Homepage",
                areaName: area,
                template: "",
                defaults: new {controller, action }
            );

            // Fallback default homepage
            routes.MapAreaRoute(
                name: "CoreHome",
                areaName: "Plato.Core",
                template: homeAlias,
                defaults: new { controller = "Home", action = "Index" }
            );

            // Unauthorized - 401
            routes.MapAreaRoute(
                name: "UnauthorizedPage",
                areaName: "Plato.Core",
                template: "denied",
                defaults: new { controller = "Home", action = "Denied" }
            );

            // Moved - 404
            routes.MapAreaRoute(
                name: "MovedPage",
                areaName: "Plato.Core",
                template: "moved",
                defaults: new { controller = "Home", action = "Moved" }
            );

            // Error page - 500
            routes.MapAreaRoute(
                name: "ErrorPage",
                areaName: "Plato.Core",
                template: "error",
                defaults: new { controller = "Home", action = "Error" }
            );

        }

    }

}