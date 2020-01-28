using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.Categories.Follow.Notifications;
using Plato.Questions.Categories.Follow.NotificationTypes;
using Plato.Questions.Categories.Follow.Subscribers;
using Plato.Questions.Categories.Follow.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Questions.Models;
using Plato.Follows.Services;
using Plato.Questions.Categories.Models;
using PlatoCore.Security.Abstractions;
using Plato.Questions.Categories.Follow.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Questions.Categories.Follow
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Question>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Question>, NotificationManager<Question>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Question>, NewQuestionEmail>();
            services.AddScoped<INotificationProvider<Question>, NewQuestionWeb>();

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