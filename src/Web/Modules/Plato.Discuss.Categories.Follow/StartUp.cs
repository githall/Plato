using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Categories.Follow.Notifications;
using Plato.Discuss.Categories.Follow.NotificationTypes;
using Plato.Discuss.Categories.Follow.Subscribers;
using Plato.Discuss.Categories.Follow.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Discuss.Models;
using Plato.Follows.Services;
using Plato.Discuss.Categories.Models;
using PlatoCore.Security.Abstractions;
using Plato.Discuss.Categories.Follow.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Categories.Follow
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Channel View providers
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Broker subscriptions
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Topic>, NewTopicEmail>();
            services.AddScoped<INotificationProvider<Topic>, NewTopicWeb>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}