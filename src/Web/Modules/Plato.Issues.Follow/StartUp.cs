using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Follow.Handlers;
using Plato.Issues.Follow.Notifications;
using Plato.Issues.Follow.NotificationTypes;
using Plato.Issues.Follow.QueryAdapters;
using Plato.Issues.Follow.Subscribers;
using Plato.Issues.Follow.ViewProviders;
using Plato.Issues.Models;
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

namespace Plato.Issues.Follow
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
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();
            
            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Comment>, NotificationManager<Comment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Comment>, NewIssueCommentEmail>();
            services.AddScoped<INotificationProvider<Comment>, NewIssueCommentWeb>();
        
            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Issue>, IssueQueryAdapter>();

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