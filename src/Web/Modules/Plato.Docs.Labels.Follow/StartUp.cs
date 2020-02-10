using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.Labels.Follow.Notifications;
using Plato.Docs.Labels.Follow.NotificationTypes;
using Plato.Docs.Labels.Follow.Subscribers;
using Plato.Docs.Labels.Follow.ViewProviders;
using Plato.Docs.Labels.Models;
using Plato.Docs.Models;
using Plato.Follows.Services;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Docs.Labels.Follow.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Docs.Labels.Follow
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
            services.AddScoped<INotificationManager<Doc>, NotificationManager<Doc>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Doc>, NewLabelEmail>();
            services.AddScoped<INotificationProvider<Doc>, NewLabelWeb>();

            // View providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Doc>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}