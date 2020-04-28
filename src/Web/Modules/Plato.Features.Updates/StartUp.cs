﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Features.Updates.Handlers;
using Plato.Features.Updates.Navigation;
using Plato.Features.Updates.Services;
using Plato.Features.Updates.ViewModels;
using Plato.Features.Updates.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Features.Updates
{
    public class Startup : StartupBase
    {
    
        public override void ConfigureServices(IServiceCollection services)
        {
            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // View providers
            services.AddScoped<IViewProviderManager<FeatureUpdatesViewModel>, ViewProviderManager<FeatureUpdatesViewModel>>();
            services.AddScoped<IViewProvider<FeatureUpdatesViewModel>, AdminViewProvider>();
       
            // Register moderation permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Update service
            services.AddScoped<IShellFeatureUpdater, ShellFeatureUpdater>();

            // Automated feature migrations
            services.AddScoped<IAutomaticFeatureMigrations, AutomaticFeatureMigrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Apply automatic feature migrations
            var automaticFeatureMigrations = serviceProvider.GetService<IAutomaticFeatureMigrations>();
            automaticFeatureMigrations.InitialMigrationsAsync()
                .GetAwaiter()
                .GetResult();

            // Index
            routes.MapAreaRoute(
                name: "FeatureUpdatesIndex",
                areaName: "Plato.Features.Updates",
                template: "admin/features/updates",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}