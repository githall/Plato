using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
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
using PlatoCore.Hosting.Abstractions;
using Plato.Site.Handlers;
using PlatoCore.Features.Abstractions;
using Plato.Site.Repositories;
using Plato.Site.Services;
using PlatoCore.Data.Migrations.Abstractions;

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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Configuration
            services.AddTransient<IConfigureOptions<PlatoSiteOptions>, PlatoSiteOptionsConfiguration>();

            // Repositories
            services.AddScoped<ISignUpRepository<SignUp>, SignUpRepository>();

            // Stores
            services.AddScoped<IPlatoSiteSettingsStore<PlatoSiteSettings>, PlatoSiteSettingsStore>();
            services.AddScoped<ISignUpStore<SignUp>, SignUpStore>();
            services.AddScoped<ISignUpManager<SignUp>, SignUpManager>();

            // Emails
            services.AddScoped<ISignUpEmails, SignUpEmails>();

            // View providers
            services.AddScoped<IViewProviderManager<PlatoSiteSettings>, ViewProviderManager<PlatoSiteSettings>>();
            services.AddScoped<IViewProvider<PlatoSiteSettings>, AdminViewProvider>();

            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

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
            // Get Started
            // -----------

            // Index
            routes.MapAreaRoute(
                name: "PlatoSiteGetStarted",
                areaName: "Plato.Site",
                template: "get-started",
                defaults: new { controller = "GetStarted", action = "Index" }
            );

            // -----------
            // Features
            // -----------

            // Index
            routes.MapAreaRoute(
                name: "PlatoSiteFeatures",
                areaName: "Plato.Site",
                template: "features",
                defaults: new { controller = "Plato", action = "Index" }
            );

            // Discuss
            routes.MapAreaRoute(
                name: "PlatoSiteDiscuss",
                areaName: "Plato.Site",
                template: "features/discuss",
                defaults: new { controller = "Plato", action = "Discuss" }
            );

            // Docs
            routes.MapAreaRoute(
                name: "PlatoSiteDocs",
                areaName: "Plato.Site",
                template: "features/docs",
                defaults: new { controller = "Plato", action = "Docs" }
            );
            
            // Articles
            routes.MapAreaRoute(
                name: "PlatoSiteArticles",
                areaName: "Plato.Site",
                template: "features/articles",
                defaults: new { controller = "Plato", action = "Articles" }
            );

            // Questions
            routes.MapAreaRoute(
                name: "PlatoSiteQuestions",
                areaName: "Plato.Site",
                template: "features/questions",
                defaults: new { controller = "Plato", action = "Questions" }
            );

            // Ideas
            routes.MapAreaRoute(
                name: "PlatoSiteIdeas",
                areaName: "Plato.Site",
                template: "features/ideas",
                defaults: new { controller = "Plato", action = "Ideas" }
            );

            // Issues
            routes.MapAreaRoute(
                name: "PlatoSiteIssues",
                areaName: "Plato.Site",
                template: "features/issues",
                defaults: new { controller = "Plato", action = "Issues" }
            );

            // Modules
            routes.MapAreaRoute(
                name: "PlatoSiteModules",
                areaName: "Plato.Site",
                template: "features/modules",
                defaults: new { controller = "Plato", action = "Modules" }
            );

            // Desktop
            routes.MapAreaRoute(
                name: "PlatoSiteDownload",
                areaName: "Plato.Site",
                template: "features/desktop",
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
                name: "PlatoSitePricingOpenSource",
                areaName: "Plato.Site",
                template: "open-source",
                defaults: new { controller = "OpenSource", action = "Index" }
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