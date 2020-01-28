using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Repositories.Metrics;
using Plato.Metrics.ActionFilters;
using Plato.Metrics.Handlers;
using Plato.Metrics.Models;
using Plato.Metrics.Repositories;
using Plato.Metrics.Services;
using Plato.Metrics.Stores;

namespace Plato.Metrics
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
            services.AddScoped<IMetricsRepository<Metric>, MetricsRepository>();
            services.AddScoped<IAggregatedMetricsRepository, AggregatedMetricsRepository>();
            
            // Stores
            services.AddScoped<IMetricsStore<Metric>, MetricsStore>();
      
            // Managers
            services.AddScoped<IMetricsManager<Metric>, MetricsManager>();

            // Action filter
            services.AddScoped<IModularActionFilter, MetricFilter>();
         
            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

       
        }

    }

}