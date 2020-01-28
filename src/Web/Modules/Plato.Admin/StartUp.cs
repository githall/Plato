using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Admin.ActionFilters;
using Plato.Admin.Models;
using Plato.Admin.Navigation;
using Plato.Admin.ViewProviders;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Admin
{
    public class Startup : StartupBase
    {
    
        public override void ConfigureServices(IServiceCollection services)
        {
            // register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminViewProvider>();
        
            // Authorization filter
            services.AddScoped<IModularActionFilter, AuthorizationFilter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "AdminIndex",
                areaName: "Plato.Admin",
                template: "admin",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}