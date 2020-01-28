using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.Follow.Handlers;
using Plato.Questions.Follow.Notifications;
using Plato.Questions.Follow.NotificationTypes;
using Plato.Questions.Follow.QueryAdapters;
using Plato.Questions.Follow.Subscribers;
using Plato.Questions.Follow.ViewProviders;
using Plato.Questions.Models;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Questions.Follow
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
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();
            services.AddScoped<IViewProviderManager<Answer>, ViewProviderManager<Answer>>();
            services.AddScoped<IViewProvider<Answer>, AnswerViewProvider>();
            
            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Answer>>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Answer>, NotificationManager<Answer>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Answer>, NewAnswerEmail>();
            services.AddScoped<INotificationProvider<Answer>, NewAnswerWeb>();
        
            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Question>, QuestionQueryAdapter>();

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