using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.GitHub.Configuration;
using Plato.GitHub.Handlers;
using Plato.GitHub.Models;
using Plato.GitHub.Navigation;
using Plato.GitHub.Stores;
using Plato.GitHub.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.GitHub
{

    public class Startup : StartupBase
    {

        public Startup()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Configuration
            services.AddTransient<IConfigureOptions<GitHubAuthenticationOptions>, GitHubAuthenticationOptionsConfiguration>();

            // Stores
            services.AddScoped<IGitHubSettingsStore<GitHubSettings>, GitHubSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<GitHubSettings>, ViewProviderManager<GitHubSettings>>();
            services.AddScoped<IViewProvider<GitHubSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
      
            routes.MapAreaRoute(
                name: "PlatoGitHubAdmin",
                areaName: "Plato.GitHub",
                template: "admin/settings/github",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}