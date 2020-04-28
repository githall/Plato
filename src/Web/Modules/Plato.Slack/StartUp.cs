﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Slack.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Slack.Stores;
using Plato.Slack.ViewProviders;
using Plato.Slack.Models;
using Plato.Slack.Navigation;
using Plato.Slack.Configuration;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Security.Abstractions;
using Plato.Slack.Services;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Slack
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
            services.AddTransient<IConfigureOptions<PlatoSlackOptions>, SlackOptionsConfiguration>();

            // Stores
            services.AddScoped<ISlackSettingsStore<PlatoSlackSettings>, SlackSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<PlatoSlackSettings>, ViewProviderManager<PlatoSlackSettings>>();
            services.AddScoped<IViewProvider<PlatoSlackSettings>, AdminViewProvider>();

            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Services
            services.AddScoped<ISlackService, SlackService>();
            
        }
        
        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoSlackAdmin",
                areaName: "Plato.Slack",
                template: "admin/settings/slack",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}