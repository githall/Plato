using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using Plato.Questions.Star.Subscribers;
using Plato.Questions.Star.ViewProviders;
using Plato.Questions.Models;
using Plato.Questions.Star.Handlers;
using Plato.Questions.Star.QueryAdapters;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Questions.Star
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
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();

            // Star subscribers
            services.AddScoped<IBrokerSubscriber, StarSubscriber>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Question>, QuestionQueryAdapter>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}