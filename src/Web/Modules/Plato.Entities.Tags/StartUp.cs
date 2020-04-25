using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Models;
using Plato.Entities.Tags.Search;
using PlatoCore.Models.Shell;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Entities.Tags
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