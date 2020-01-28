using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Settings.Handlers;
using Plato.Settings.Models;
using Plato.Settings.Navigation;
using Plato.Settings.ViewProviders;

namespace Plato.Settings
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

            // Register setup events
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<SettingsIndex>, ViewProviderManager<SettingsIndex>>();
            services.AddScoped<IViewProvider<SettingsIndex>, AdminViewProvider>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "PlatoSettingsAdmin",
                areaName: "Plato.Settings",
                template: "admin/settings/general",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}