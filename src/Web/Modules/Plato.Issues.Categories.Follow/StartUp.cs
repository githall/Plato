﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Categories.Follow.Notifications;
using Plato.Issues.Categories.Follow.NotificationTypes;
using Plato.Issues.Categories.Follow.Subscribers;
using Plato.Issues.Categories.Follow.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Issues.Models;
using Plato.Follows.Services;
using Plato.Issues.Categories.Models;
using PlatoCore.Security.Abstractions;
using Plato.Issues.Categories.Follow.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.Categories.Follow
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Issue>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Issue>, NotificationManager<Issue>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Issue>, NewIssueEmail>();
            services.AddScoped<INotificationProvider<Issue>, NewIssueWeb>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}