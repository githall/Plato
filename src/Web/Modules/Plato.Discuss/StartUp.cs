using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Discuss.Handlers;
using Plato.Discuss.Assets;
using Plato.Discuss.Badges;
using Plato.Discuss.Models;
using Plato.Discuss.Navigation;
using Plato.Discuss.Services;
using Plato.Discuss.Subscribers;
using Plato.Discuss.Tasks;
using Plato.Discuss.ViewProviders;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Discuss.NotificationTypes;
using Plato.Discuss.Notifications;
using Plato.Entities.Models;
using Plato.Entities.Search;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Models.Reputations;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Discuss
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, HomeMenu>();
            services.AddScoped<INavigationProvider, SearchMenu>();
            services.AddScoped<INavigationProvider, PostMenu>();
            services.AddScoped<INavigationProvider, UserEntitiesMenu>();
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Topic>, EntityRepository<Topic>>();
            services.AddScoped<IEntityStore<Topic>, EntityStore<Topic>>();
            services.AddScoped<IEntityManager<Topic>, EntityManager<Topic>>();
            services.AddScoped<IEntityReplyRepository<Reply>, EntityReplyRepository<Reply>>();
            services.AddScoped<IEntityReplyStore<Reply>, EntityReplyStore<Reply>>();
            services.AddScoped<IEntityReplyManager<Reply>, EntityReplyManager<Reply>>();

            // Register data access
            services.AddScoped<IPostManager<Topic>, TopicManager>();
            services.AddScoped<IPostManager<Reply>, ReplyManager>();

            // Entity services - transient as they contains action
            // delegates that can change state several times per request
            services.AddTransient<IEntityService<Topic>, EntityService<Topic>>();
            services.AddTransient<IEntityReplyService<Reply>, EntityReplyService<Reply>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Topic>, EntityViewIncrementer<Topic>>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register reputation provider
            services.AddScoped<IReputationsProvider<Reputation>, Reputations>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();
         
            // Register admin view providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminViewProvider>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();

            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, TopicSubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, ReplySubscriber<Reply>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TopicBadges>();
            services.AddScoped<IBadgesProvider<Badge>, ReplyBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TopicBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, ReplyBadgesAwarder>();

            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Topic>>, NotificationManager<ReportSubmission<Topic>>>();
            services.AddScoped<INotificationManager<ReportSubmission<Reply>>, NotificationManager<ReportSubmission<Reply>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Topic>>, TopicReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Topic>>, TopicReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<Reply>>, ReplyReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Reply>>, ReplyReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Topic>, ReportTopicManager>();
            services.AddScoped<IReportEntityManager<Reply>, ReportReplyManager>();

            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Topic>, FederatedQueryManager<Topic>>();
            services.AddScoped<IFederatedQueryProvider<Topic>, EntityQueries<Topic>>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<Topic>, QueryAdapterManager<Topic>>();
           
            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Index
            routes.MapAreaRoute(
                name: "Discuss",
                areaName: "Plato.Discuss",
                template: "discuss/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Popular
            routes.MapAreaRoute(
                name: "DiscussPopular",
                areaName: "Plato.Discuss",
                template: "discuss/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "DiscussTopic",
                areaName: "Plato.Discuss",
                template: "discuss/t/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "DiscussNewTopic",
                areaName: "Plato.Discuss",
                template: "discuss/new/{channel:int?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // Edit Entity
            routes.MapAreaRoute(
                name: "DiscussEditTopic",
                areaName: "Plato.Discuss",
                template: "discuss/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "DiscussDisplayReply",
                areaName: "Plato.Discuss",
                template: "discuss/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "DiscussReport",
                areaName: "Plato.Discuss",
                template: "discuss/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );

            // User Index
            routes.MapAreaRoute(
                name: "DiscussUser",
                areaName: "Plato.Discuss",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/topics/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

        }

    }

}