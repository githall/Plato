using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Providers;
using PlatoCore.Data.Tracing.Abstractions;

namespace PlatoCore.Data.Tracing.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataTracing(this IServiceCollection services)
        {
            services.AddScoped<IDbTraceState, DefaultDbTraceState>();
            services.AddScoped<IDbTracer<SqlProvider>, DefaultDbTracer<SqlProvider>>();
            return services;
        }

    }

}
