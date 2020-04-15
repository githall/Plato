using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.SetUp;
using Plato.Tenants.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Tenants.Navigation;
using PlatoCore.Navigation.Abstractions;
using Plato.Tenants.ViewProviders;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;

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

            // Set-up event handler
            //services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Feature installation event handler
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Admin view provider
            services.AddScoped<IViewProviderManager<ShellSettings>, ViewProviderManager<ShellSettings>>();
            services.AddScoped<IViewProvider<ShellSettings>, AdminViewProvider>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}