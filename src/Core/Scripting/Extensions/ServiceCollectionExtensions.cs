using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Scripting.Abstractions;

namespace PlatoCore.Scripting.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoScripting(
            this IServiceCollection services)
        {

            services.TryAddScoped<IScriptManager, ScriptManager>();
            
            return services;

        }


    }
}
