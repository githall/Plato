using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Messaging.Abstractions;


namespace PlatoCore.Messaging.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoMessaging(
            this IServiceCollection services)
        {

            services.AddSingleton<IBroker, Broker>();

            return services;

        }


    }

}
