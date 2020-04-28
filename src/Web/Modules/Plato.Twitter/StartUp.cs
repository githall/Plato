using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Twitter.Configuration;
using Plato.Twitter.Handlers;
using Plato.Twitter.Models;
using Plato.Twitter.Navigation;
using Plato.Twitter.Stores;
using Plato.Twitter.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Twitter
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

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Configuration
            services.AddTransient<IConfigureOptions<PlatoTwitterOptions>, PlatoTwitterOptionsConfiguration>();

            // Stores
            services.AddScoped<ITwitterSettingsStore<PlatoTwitterSettings>, TwitterSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<PlatoTwitterSettings>, ViewProviderManager<PlatoTwitterSettings>>();
            services.AddScoped<IViewProvider<PlatoTwitterSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoTwitterAdmin",
                areaName: "Plato.Twitter",
                template: "admin/settings/twitter",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}