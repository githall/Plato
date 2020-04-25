using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Categories.Roles.QueryAdapters;
using Plato.Entities.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Entities.Categories.Roles
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

            // Query adapters to limit access by role
            services.AddScoped<IQueryAdapterProvider<Entity>, EntityQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<FeatureEntityCount>, FeatureEntityCountQueryAdapter>();

        }

    }

}