using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Tasks.Abstractions;

namespace PlatoCore.Tasks.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTasks(
            this IServiceCollection services)
        {

            // Background tasks
            services.TryAddSingleton<IBackgroundTaskManager, BackgroundTaskManager>();
            services.TryAddSingleton<ISafeTimerFactory, SafeTimerFactory>();

            // Deferred tasks
            services.TryAddSingleton<IDeferredTaskState, SingletonTaskState>();
            services.TryAddScoped<IDeferredTaskManager, DeferredTaskManager>();

            return services;

        }

    }

}
