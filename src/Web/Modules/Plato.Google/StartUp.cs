using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Google.Configuration;
using Plato.Google.Handlers;
using Plato.Google.Models;
using Plato.Google.Navigation;
using Plato.Google.Stores;
using Plato.Google.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Google
{

    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Configuration
            services.AddTransient<IConfigureOptions<PlatoGoogleOptions>, PlatoGoogleOptionsConfiguration>();

            // Stores
            services.AddScoped<IGoogleSettingsStore<PlatoGoogleSettings>, GoogleSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<PlatoGoogleSettings>, ViewProviderManager<PlatoGoogleSettings>>();
            services.AddScoped<IViewProvider<PlatoGoogleSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
      
            routes.MapAreaRoute(
                name: "PlatoGoogleAdmin",
                areaName: "Plato.Google",
                template: "admin/settings/google",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}