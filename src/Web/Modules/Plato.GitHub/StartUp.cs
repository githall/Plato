﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.GitHub.Configuration;
using Plato.GitHub.Handlers;
using Plato.GitHub.Models;
using Plato.GitHub.Navigation;
using Plato.GitHub.Stores;
using Plato.GitHub.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.GitHub
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
            services.AddTransient<IConfigureOptions<PlatoGitHubOptions>, PlatoGitHubOptionsConfiguration>();

            // Stores
            services.AddScoped<IGitHubSettingsStore<PlatoGitHubSettings>, GitHubSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<PlatoGitHubSettings>, ViewProviderManager<PlatoGitHubSettings>>();
            services.AddScoped<IViewProvider<PlatoGitHubSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
      
            routes.MapAreaRoute(
                name: "PlatoGitHubAdmin",
                areaName: "Plato.GitHub",
                template: "admin/settings/github",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}