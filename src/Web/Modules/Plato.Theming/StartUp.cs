using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Theming.Abstractions;
using Plato.Theming.Handlers;
using Plato.Theming.Navigation;
using Plato.Theming.Models;
using Plato.Theming.Services;
using Plato.Theming.ViewProviders;

namespace Plato.Theming
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<ThemeAdmin>, ViewProviderManager<ThemeAdmin>>();
            services.AddScoped<IViewProvider<ThemeAdmin>, AdminViewProvider>();

            // Replace dummy site theme services with tenant specific implementations
            services.Replace<ISiteThemeLoader, SiteThemeLoader>(ServiceLifetime.Scoped);
            services.Replace<ISiteThemeFileManager, SiteThemeFileManager>(ServiceLifetime.Scoped);
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }
}