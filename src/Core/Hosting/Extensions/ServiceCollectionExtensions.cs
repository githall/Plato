using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;

namespace PlatoCore.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
 
        public static IServiceCollection AddPlatoDefaultHost(
            this IServiceCollection services)
        {
            services.AddSingleton<DefaultPlatoHost>();
            services.AddSingleton<IPlatoHost>(sp => sp.GetRequiredService<DefaultPlatoHost>());
            return services;
        }


    }

}
