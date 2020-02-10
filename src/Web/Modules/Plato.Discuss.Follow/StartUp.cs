using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Follow.Handlers;
using Plato.Discuss.Follow.Notifications;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Discuss.Follow.QueryAdapters;
using Plato.Discuss.Follow.Subscribers;
using Plato.Discuss.Follow.ViewProviders;
using Plato.Discuss.Models;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss.Follow
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

            // View providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();
            
            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Reply>, NotificationManager<Reply>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Reply>, NewReplyEmail>();
            services.AddScoped<INotificationProvider<Reply>, NewReplyWeb>();
        
            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Topic>, TopicQueryAdapter>();

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