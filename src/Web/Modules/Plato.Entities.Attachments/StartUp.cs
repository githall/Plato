using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Attachments.Search;
using Plato.Entities.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using Plato.Entities.Attachments.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Assets.Abstractions;

namespace Plato.Entities.Attachments
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

       
            // Federated search
            //services.AddScoped<IFederatedQueryManager<Entity>, FederatedQueryManager<Entity>>();
            //services.AddScoped<IFederatedQueryProvider<Entity>, EntityQueries<Entity>>();
            //services.AddScoped<IFederatedQueryManager<FeatureEntityCount>, FederatedQueryManager<FeatureEntityCount>>();
            //services.AddScoped<IFederatedQueryProvider<FeatureEntityCount>, FeatureEntityCountQueries<FeatureEntityCount>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}