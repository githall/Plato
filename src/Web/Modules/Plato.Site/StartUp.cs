using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Site.Assets;
using Plato.Site.Navigation;
using PlatoCore.Navigation.Abstractions;
using Plato.Site.Configuration;
using Microsoft.Extensions.Options;
using Plato.Site.Models;
using Plato.Site.Stores;
using PlatoCore.Layout.ViewProviders;
using Plato.Site.ViewProviders;

namespace Plato.Site
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

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Configuration
            services.AddTransient<IConfigureOptions<PlatoSiteOptions>, PlatoSiteOptionsConfiguration>();

            // Stores
            services.AddScoped<IPlatoSiteSettingsStore<PlatoSiteSettings>, PlatoSiteSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<PlatoSiteSettings>, ViewProviderManager<PlatoSiteSettings>>();
            services.AddScoped<IViewProvider<PlatoSiteSettings>, AdminViewProvider>();

            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

            // Permissions provider
            //services.AddScoped<IPermissionsProvider<Permission>, Permissions>();            

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            
            // About
            routes.MapAreaRoute(
                name: "PlatoSiteAbout",
                areaName: "Plato.Site",
                template: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            // Features
            routes.MapAreaRoute(
                name: "PlatoSiteFeatures",
                areaName: "Plato.Site",
                template: "features",
                defaults: new { controller = "Home", action = "Features" }
            );

            // Modules
            routes.MapAreaRoute(
                name: "PlatoSiteModules",
                areaName: "Plato.Site",
                template: "modules",
                defaults: new { controller = "Home", action = "Modules" }
            );

            // Download
            routes.MapAreaRoute(
                name: "PlatoSiteDownload",
                areaName: "Plato.Site",
                template: "download",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Pricing
            routes.MapAreaRoute(
                name: "PlatoSitePricing",
                areaName: "Plato.Site",
                template: "pricing",
                defaults: new { controller = "Home", action = "Pricing" }
            );

            // Support Options
            routes.MapAreaRoute(
                name: "PlatoSiteSupport",
                areaName: "Plato.Site",
                template: "support/options",
                defaults: new { controller = "Home", action = "SupportOptions" }
            );
            
            // Contact
            routes.MapAreaRoute(
                name: "PlatoSiteContact",
                areaName: "Plato.Site",
                template: "contact",
                defaults: new { controller = "Home", action = "Contact" }
            );

            // Privacy
            routes.MapAreaRoute(
                name: "PlatoSitePrivacy",
                areaName: "Plato.Site",
                template: "privacy",
                defaults: new { controller = "Home", action = "Privacy" }
            );

            // Terms
            routes.MapAreaRoute(
                name: "PlatoSiteTerms",
                areaName: "Plato.Site",
                template: "terms",
                defaults: new { controller = "Home", action = "Terms" }
            );

            // Admin Settings
            routes.MapAreaRoute(
                 name: "PlatoSiteSettingsAdmin",
                 areaName: "Plato.Site",
                 template: "admin/settings/site",
                 defaults: new { controller = "Admin", action = "Index" }
            );

            // Catch All
            routes.MapAreaRoute(
                name: "PlatoSiteIndex",
                areaName: "Plato.Site",
                template: "site/{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}