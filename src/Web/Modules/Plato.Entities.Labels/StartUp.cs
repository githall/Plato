using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Labels.Search;
using Plato.Entities.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Entities.Labels
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

            // Federated search
            services.AddScoped<IFederatedQueryManager<Entity>, FederatedQueryManager<Entity>>();
            services.AddScoped<IFederatedQueryProvider<Entity>, EntityQueries<Entity>>();
            services.AddScoped<IFederatedQueryManager<FeatureEntityCount>, FederatedQueryManager<FeatureEntityCount>>();
            services.AddScoped<IFederatedQueryProvider<FeatureEntityCount>, FeatureEntityCountQueries<FeatureEntityCount>>();

        }

    }

}