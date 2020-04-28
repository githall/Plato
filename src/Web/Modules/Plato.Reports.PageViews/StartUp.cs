using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using Plato.Reports.PageViews.Navigation;
using Plato.Reports.PageViews.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Reports.PageViews.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Reports.PageViews
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
            services.AddScoped<IViewProviderManager<PageViewIndex>, ViewProviderManager<PageViewIndex>>();
            services.AddScoped<IViewProvider<PageViewIndex>, AdminViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Page Views
            routes.MapAreaRoute(
                name: "ReportsPageViews",
                areaName: "Plato.Reports.PageViews",
                template: "admin/reports/page-views/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}