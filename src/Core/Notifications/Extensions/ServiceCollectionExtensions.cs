using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Notifications.Abstractions;

namespace PlatoCore.Notifications.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNotifications(
            this IServiceCollection services)
        {

            // Type Manager
            services.TryAddScoped<INotificationTypeManager, NotificationTypeManager>();

            // The below implementations are replaced with real
            // implementations within the Plato.Notifications module
      
            // Dummy user notification implementation
            services.TryAddScoped<IUserNotificationsManager<UserNotification>, DummyUserNotificationsManager>();

            // Dummy user notification type defaults implementation
            services.AddScoped<IUserNotificationTypeDefaults, DummyUserNotificationTypeDefaults>();

            return services;

        }

    }

}