using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ActionFilters;
using Plato.Entities.Metrics.ActionFilters;
using Plato.Entities.Metrics.Handlers;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Metrics.Services;
using Plato.Entities.Metrics.Stores;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Entities.Metrics
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

            // Repositories
            services.AddScoped<IEntityMetricsRepository<EntityMetric>, EntityMetricsRepository>();
            services.AddScoped<IAggregatedEntityMetricsRepository, AggregatedEntityMetricsRepository>();
            
            // Stores
            services.AddScoped<IEntityMetricsStore<EntityMetric>, EntityMetricsStore>();
      
            // Managers
            services.AddScoped<IEntityMetricsManager<EntityMetric>, EntityMetricsManager>();

            // Action filter
            services.AddScoped<IModularActionFilter, EntityMetricFilter>();
        
            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

    }

}