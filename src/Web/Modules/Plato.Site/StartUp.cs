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
using PlatoCore.Layout.ViewProviders.Abstractions;
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

            // -----------
            // Home
            // -----------

            // Index
            routes.MapAreaRoute(
                name: "PlatoSiteAbout",
                areaName: "Plato.Site",
                template: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            // -----------
            // Plato
            // -----------

            // Index
            routes.MapAreaRoute(
                name: "PlatoSiteFeatures",
                areaName: "Plato.Site",
                template: "plato",
                defaults: new { controller = "Plato", action = "Features" }
            );

            // Discuss
            routes.MapAreaRoute(
                name: "PlatoSiteDiscuss",
                areaName: "Plato.Site",
                template: "plato/discuss",
                defaults: new { controller = "Plato", action = "Discuss" }
            );

            // Docs
            routes.MapAreaRoute(
                name: "PlatoSiteDocs",
                areaName: "Plato.Site",
                template: "plato/docs",
                defaults: new { controller = "Plato", action = "Docs" }
            );
            
            // Articles
            routes.MapAreaRoute(
                name: "PlatoSiteArticles",
                areaName: "Plato.Site",
                template: "plato/articles",
                defaults: new { controller = "Plato", action = "Articles" }
            );

            // Questions
            routes.MapAreaRoute(
                name: "PlatoSiteQuestions",
                areaName: "Plato.Site",
                template: "plato/questions",
                defaults: new { controller = "Plato", action = "Questions" }
            );

            // Ideas
            routes.MapAreaRoute(
                name: "PlatoSiteIdeas",
                areaName: "Plato.Site",
                template: "plato/ideas",
                defaults: new { controller = "Plato", action = "Ideas" }
            );

            // Issues
            routes.MapAreaRoute(
                name: "PlatoSiteIssues",
                areaName: "Plato.Site",
                template: "plato/issues",
                defaults: new { controller = "Plato", action = "Issues" }
            );

            // Modules
            routes.MapAreaRoute(
                name: "PlatoSiteModules",
                areaName: "Plato.Site",
                template: "plato/modules",
                defaults: new { controller = "Plato", action = "Modules" }
            );

            // Desktop
            routes.MapAreaRoute(
                name: "PlatoSiteDownload",
                areaName: "Plato.Site",
                template: "plato/desktop",
                defaults: new { controller = "Plato", action = "Desktop" }
            );

            // -----------
            // Pricing
            // -----------

            routes.MapAreaRoute(
                name: "PlatoSitePricing",
                areaName: "Plato.Site",
                template: "pricing",
                defaults: new { controller = "Pricing", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "PlatoSitePricingFullyManaged",
                areaName: "Plato.Site",
                template: "pricing/managed",
                defaults: new { controller = "Pricing", action = "FullyManaged" }
            );


            // -----------
            // Support
            // -----------

            // Support Options
            routes.MapAreaRoute(
                name: "PlatoSiteSupport",
                areaName: "Plato.Site",
                template: "support-options",
                defaults: new { controller = "Support", action = "Index" }
            );

            // -----------
            // Company
            // -----------

            // Contact
            routes.MapAreaRoute(
                name: "PlatoSiteContact",
                areaName: "Plato.Site",
                template: "contact",
                defaults: new { controller = "Company", action = "Contact" }
            );


            // -----------
            // Legal
            // -----------

            // License
            routes.MapAreaRoute(
                name: "PlatoSiteLicense",
                areaName: "Plato.Site",
                template: "legal/license",
                defaults: new { controller = "Legal", action = "License" }
            );

            // Privacy
            routes.MapAreaRoute(
                name: "PlatoSitePrivacy",
                areaName: "Plato.Site",
                template: "legal/privacy",
                defaults: new { controller = "Legal", action = "Privacy" }
            );

            // Terms
            routes.MapAreaRoute(
                name: "PlatoSiteTerms",
                areaName: "Plato.Site",
                template: "legal/terms",
                defaults: new { controller = "Legal", action = "Terms" }
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