using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using Plato.Issues.Star.Subscribers;
using Plato.Issues.Star.ViewProviders;
using Plato.Issues.Models;
using Plato.Issues.Star.Handlers;
using Plato.Issues.Star.QueryAdapters;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Issues.Star
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
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();

            // Star subscribers
            services.AddScoped<IBrokerSubscriber, StarSubscriber>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Issue>, IssueQueryAdapter>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}