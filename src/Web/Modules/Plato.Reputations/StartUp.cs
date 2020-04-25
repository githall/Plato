using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using Plato.Reputations.Handlers;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Tasks.Abstractions;
using Plato.Reputations.Tasks;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Reputations
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
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Points & rank aggregator background tasks
            services.AddScoped<IBackgroundTaskProvider, UserRankAggregator>();
            services.AddScoped<IBackgroundTaskProvider, UserReputationAggregator>();

        }

    }

}