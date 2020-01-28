using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Schemas.Abstractions;

namespace PlatoCore.Data.Schemas.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataSchemas(
            this IServiceCollection services)
        {
                        
            services.AddTransient<ISchemaManager, SchemaManager>();
            services.AddTransient<ISchemaBuilder, SchemaBuilder>();

            return services;
        }


    }
}
