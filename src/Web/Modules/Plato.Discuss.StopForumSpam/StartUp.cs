using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Discuss.StopForumSpam.Notifications;
using Plato.Discuss.StopForumSpam.NotificationTypes;
using Plato.Discuss.StopForumSpam.SpamOperators;
using Plato.Discuss.StopForumSpam.ViewProviders;
using Plato.Discuss.Models;
using PlatoCore.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Discuss.StopForumSpam.Handlers;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Discuss.StopForumSpam.Navigation;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.StopForumSpam
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

            // Navigation providers
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Topic>, SpamOperatorManager<Topic>>();
            services.AddScoped<ISpamOperatorManager<Reply>, SpamOperatorManager<Reply>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Topic>, TopicOperator>();
            services.AddScoped<ISpamOperatorProvider<Reply>, ReplyOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();
            services.AddScoped<INotificationManager<Reply>, NotificationManager<Reply>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Topic>, TopicSpamWeb>();
            services.AddScoped<INotificationProvider<Topic>, TopicSpamEmail>();
            services.AddScoped<INotificationProvider<Reply>, ReplySpamWeb>();
            services.AddScoped<INotificationProvider<Reply>, ReplySpamEmail>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "DiscussSpammerIndex",
                areaName: "Plato.Discuss.StopForumSpam",
                template: "discuss/t/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "DiscussAddSpammer",
                areaName: "Plato.Discuss.StopForumSpam",
                template: "discuss/t/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );

        }

    }

}