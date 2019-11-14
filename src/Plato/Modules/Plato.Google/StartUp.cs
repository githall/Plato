﻿using System;
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
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Google
{

    public class Startup : StartupBase
    {

        public Startup()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Configuration
            services.AddTransient<IConfigureOptions<GoogleOptions>, GoogleOptionsConfiguration>();

            // Stores
            services.AddScoped<IGoogleSettingsStore<GoogleSettings>, GoogleSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<GoogleSettings>, ViewProviderManager<GoogleSettings>>();
            services.AddScoped<IViewProvider<GoogleSettings>, AdminViewProvider>();
            
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