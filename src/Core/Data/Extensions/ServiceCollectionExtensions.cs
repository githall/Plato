using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Providers;
using PlatoCore.Data.Abstractions;
using PlatoCore.Data.Schemas.Extensions;
using PlatoCore.Data.Tracing.Extensions;
using PlatoCore.Data.Migrations.Extensions;

namespace PlatoCore.Data.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDbContext(
            this IServiceCollection services)
        {

            // Add default data options and data context
            // DbContextOptions is overridden for each tenant within ShellContainerFactory
            services.AddScoped<IDbContextOptions, DbContextOptions>();

            // Provider
            services.AddScoped<IDataProvider, SqlProvider>();

            // Data access
            services.AddScoped<IDbContext, DbContext>();            
            services.AddTransient<IDbHelper, DbHelper>();

            // Add schema
            services.AddDataSchemas();

            // Add migrations 
            services.AddDataMigrations();

            // Add tracing 
            services.AddDataTracing();

            return services;

        }

    }

}
