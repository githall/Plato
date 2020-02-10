using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Follow.Notifications;
using Plato.Discuss.Tags.Follow.NotificationTypes;
using Plato.Follows.Services;
using Plato.Discuss.Tags.Follow.Subscribers;
using Plato.Discuss.Tags.Follow.ViewProviders;
using Plato.Discuss.Tags.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Features.Abstractions;
using Plato.Discuss.Tags.Follow.Handlers;

namespace Plato.Discuss.Tags.Follow
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

            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();
            services.AddScoped<INotificationManager<Reply>, NotificationManager<Reply>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Topic>, NewTagEmail>();
            services.AddScoped<INotificationProvider<Topic>, NewTagWeb>();
            services.AddScoped<INotificationProvider<Reply>, NewTagReplyEmail>();
            services.AddScoped<INotificationProvider<Reply>, NewTagReplyWeb>();

            // View providers
            services.AddScoped<IViewProviderManager<Tag>, ViewProviderManager<Tag>>();
            services.AddScoped<IViewProvider<Tag>, TagViewProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

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