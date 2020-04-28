﻿using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.Categories.Follow.Notifications;
using Plato.Ideas.Categories.Follow.NotificationTypes;
using Plato.Ideas.Categories.Follow.Subscribers;
using Plato.Ideas.Categories.Follow.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Ideas.Models;
using Plato.Follows.Services;
using Plato.Ideas.Categories.Models;
using PlatoCore.Security.Abstractions;
using Plato.Ideas.Categories.Follow.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Ideas.Categories.Follow
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Idea>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Idea>, NotificationManager<Idea>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Idea>, NewIdeaEmail>();
            services.AddScoped<INotificationProvider<Idea>, NewIdeaWeb>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}