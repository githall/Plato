using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Facebook.Configuration;
using Plato.Facebook.Handlers;
using Plato.Facebook.Models;
using Plato.Facebook.Navigation;
using Plato.Facebook.Stores;
using Plato.Facebook.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Facebook
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
            services.AddTransient<IConfigureOptions<PlatoFacebookOptions>, PlatoFacebookOptionsConfiguration>();

            // Stores
            services.AddScoped<IFacebookSettingsStore<PlatoFacebookSettings>, FacebookSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<PlatoFacebookSettings>, ViewProviderManager<PlatoFacebookSettings>>();
            services.AddScoped<IViewProvider<PlatoFacebookSettings>, AdminViewProvider>();

            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoFacebookAdmin",
                areaName: "Plato.Facebook",
                template: "admin/settings/facebook",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}