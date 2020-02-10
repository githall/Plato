using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Labels.Follow.Notifications;
using Plato.Issues.Labels.Follow.NotificationTypes;
using Plato.Issues.Labels.Follow.Subscribers;
using Plato.Issues.Labels.Follow.ViewProviders;
using Plato.Issues.Labels.Models;
using Plato.Issues.Models;
using Plato.Follows.Services;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Issues.Labels.Follow.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Issues.Labels.Follow
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
            services.AddScoped<INotificationManager<Issue>, NotificationManager<Issue>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Issue>, NewLabelEmail>();
            services.AddScoped<INotificationProvider<Issue>, NewLabelWeb>();

            // View providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Issue>>();

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