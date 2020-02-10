using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Admin.Models;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using Plato.Reports.Assets;
using Plato.Reports.Models;
using Plato.Reports.Navigation;
using Plato.Reports.Services;
using Plato.Reports.ViewProviders;

namespace Plato.Reports
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

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register admin view providers
            services.AddScoped<IViewProviderManager<ReportIndex>, ViewProviderManager<ReportIndex>>();
            services.AddScoped<IViewProvider<ReportIndex>, AdminViewProvider>();
        
            // Register admin index View providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminIndexViewProvider>();

            // Services
            services.AddScoped<IDateRangeStorage, RouteValueDateRangeStorage>();

            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Index
            routes.MapAreaRoute(
                name: "ReportsIndex",
                areaName: "Plato.Reports",
                template: "admin/reports",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}