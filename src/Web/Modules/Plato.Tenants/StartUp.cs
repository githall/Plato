using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Tenants.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Tenants.Navigation;
using PlatoCore.Navigation.Abstractions;
using Plato.Tenants.ViewProviders;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Tenants.Services;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Tenants
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

            // View providers
            services.AddScoped<IViewProviderManager<ShellSettings>, ViewProviderManager<ShellSettings>>();
            services.AddScoped<IViewProvider<ShellSettings>, AdminViewProvider>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Services
            services.AddScoped<ITenantSetUpService, TenantSetUpService>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "TenantsIndex",
                areaName: "Plato.Tenants",
                template: "admin/tenants",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Add
            routes.MapAreaRoute(
                name: "TenantsCreate",
                areaName: "Plato.Tenants",
                template: "admin/tenants/add",
                defaults: new { controller = "Admin", action = "Create" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "TenantsEdit",
                areaName: "Plato.Tenants",
                template: "admin/tenants/edit/{id}",
                defaults: new { controller = "Admin", action = "Edit" }
            );

            // Delete
            routes.MapAreaRoute(
                name: "TenantsDelete",
                areaName: "Plato.Tenants",
                template: "admin/tenants/delete/{id}",
                defaults: new { controller = "Admin", action = "Delete" }
            );
        }

    }

}