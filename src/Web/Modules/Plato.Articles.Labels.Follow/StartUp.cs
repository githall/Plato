using Microsoft.Extensions.DependencyInjection;
using Plato.Articles.Labels.Follow.Notifications;
using Plato.Articles.Labels.Follow.NotificationTypes;
using Plato.Articles.Labels.Follow.Subscribers;
using Plato.Articles.Labels.Follow.ViewProviders;
using Plato.Articles.Labels.Models;
using Plato.Articles.Models;
using Plato.Follows.Services;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Articles.Labels.Follow.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Articles.Labels.Follow
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

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Article>, NotificationManager<Article>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Article>, NewLabelEmail>();
            services.AddScoped<INotificationProvider<Article>, NewLabelWeb>();

            // View providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Article>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}