using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;

namespace PlatoCore.Badges.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoBadges(
            this IServiceCollection services)
        {

            services.AddScoped<IBadgesManager<Badge>, BadgesManager<Badge>>();

            return services;

        }


    }
}
