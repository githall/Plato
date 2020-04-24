using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Navigation.Abstractions;

namespace PlatoCore.Navigation.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddShellNavigation(
            this IServiceCollection services)
        {

            services.TryAddScoped<INavigationManager, NavigationManager>();
            services.TryAddScoped<IBreadCrumbManager, BreadCrumbManager>();

            return services;

        }
        
    }

}
