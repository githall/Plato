using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using Plato.Reports.FeatureViews.Navigation;
using Plato.Reports.FeatureViews.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Reports.FeatureViews.Models;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Reports.FeatureViews
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register report page views view providers
            services.AddScoped<IViewProviderManager<FeatureViewIndex>, ViewProviderManager<FeatureViewIndex>>();
            services.AddScoped<IViewProvider<FeatureViewIndex>, AdminViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Page Views
            routes.MapAreaRoute(
                name: "ReportsFeatureViews",
                areaName: "Plato.Reports.FeatureViews",
                template: "admin/reports/feature-views/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}