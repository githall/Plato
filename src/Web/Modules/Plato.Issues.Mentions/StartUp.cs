﻿using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Mentions.Notifications;
using Plato.Issues.Mentions.NotificationTypes;
using Plato.Issues.Mentions.Subscribers;
using Plato.Issues.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.Mentions
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
                                         
            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Issue>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Issue>, NotificationManager<Issue>>();
            services.AddScoped<INotificationManager<Comment>, NotificationManager<Comment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Issue>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Issue>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<Comment>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<Comment>, NewReplyMentionEmail>();

        }

    }

}