using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace PlatoCore.Reputations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoReputations(
            this IServiceCollection services)
        {
            
            // User reputation provider
            //services.TryAddScoped<IReputationsManager<Reputation>, ReputationsManager<Reputation>>();

            // User reputation awarder
            //services.TryAddScoped<IUserReputationAwarder, UserReputationAwarder>();
            
            // User reputation manager
            //services.TryAddScoped<IUserReputationManager<UserReputation>, UserReputationManager>();

            return services;

        }


    }
}
