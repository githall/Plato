using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Site.Demo.Handlers;
using Plato.Site.Demo.Navigation;
using Plato.Site.Demo.Configuration;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.Stores;
using Plato.Site.Demo.ViewProviders;
using PlatoCore.Models.Users;
using Plato.Site.Demo.Services;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Site.Demo
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
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Configuration
            services.AddTransient<IConfigureOptions<DemoOptions>, DemoOptionsConfiguration>();

            // Stores
            services.AddScoped<IDemoSettingsStore<DemoSettings>, DemoSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<DemoSettings>, ViewProviderManager<DemoSettings>>();
            services.AddScoped<IViewProvider<DemoSettings>, AdminViewProvider>();
            
            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

            // Services
            services.AddScoped<ISampleEntitiesService, SampleEntitiesService>();
            services.AddScoped<ISampleUsersService, SampleUsersService>();
            services.AddScoped<ISampleCategoriesService, SampleCategoriesService>();
            services.AddScoped<ISampleLabelsService, SampleLabelsService>();
            services.AddScoped<ISampleTagsService, SampleTagsService>();
            services.AddScoped<ISampleEntityTagsService, SampleEntityTagsService>();
            services.AddScoped<ISampleEntityLabelsService, SampleEntityLabelsService>();
            services.AddScoped<ISampleEntityCategoriesService, SampleEntityCategoriesService>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Admin
            routes.MapAreaRoute(
                name: "PlatoSiteDemoAdmin",
                areaName: "Plato.Site.Demo",
                template: "admin/settings/demo",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Login
            routes.MapAreaRoute(
                name: "PlatoSiteDemoLogin",
                areaName: "Plato.Site.Demo",
                template: "demo/login",
                defaults: new { controller = "Home", action = "Login" }
            );

        }

    }

}