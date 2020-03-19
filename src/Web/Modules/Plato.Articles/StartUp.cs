﻿using System;
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
using Plato.Articles.Handlers;
using Plato.Articles.Assets;
using Plato.Articles.Badges;
using Plato.Articles.Models;
using Plato.Articles.Navigation;
using Plato.Articles.Notifications;
using Plato.Articles.NotificationTypes;
using Plato.Articles.Services;
using Plato.Articles.Subscribers;
using Plato.Articles.Tasks;
using Plato.Articles.ViewProviders;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Search;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Models.Reputations;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Articles
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
            services.AddScoped<INavigationProvider, ArticleMenu>();
            services.AddScoped<INavigationProvider, ArticleCommentMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Article>, EntityRepository<Article>>();
            services.AddScoped<IEntityStore<Article>, EntityStore<Article>>();
            services.AddScoped<IEntityManager<Article>, EntityManager<Article>>();

            // Repositories
            services.AddScoped<IEntityReplyRepository<Comment>, EntityReplyRepository<Comment>>();
            services.AddScoped<IEntityReplyStore<Comment>, EntityReplyStore<Comment>>();
            services.AddScoped<IEntityReplyManager<Comment>, EntityReplyManager<Comment>>();

            //  Post managers
            services.AddScoped<IPostManager<Article>, ArticleManager>();
            services.AddScoped<IPostManager<Comment>, ReplyManager>();

            // Entity services - transient as they contains action
            // delegates that can change state several times per request
            services.AddTransient<IEntityService<Article>, EntityService<Article>>();
            services.AddTransient<IEntityReplyService<Comment>, EntityReplyService<Comment>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Article>, EntityViewIncrementer<Article>>();

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
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();

            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, ArticleSubscriber<Article>>();
            services.AddScoped<IBrokerSubscriber, CommentSubscriber<Comment>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ArticleBadges>();
            services.AddScoped<IBadgesProvider<Badge>, CommentBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, ArticleBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, CommentBadgesAwarder>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Article>>, NotificationManager<ReportSubmission<Article>>>();
            services.AddScoped<INotificationManager<ReportSubmission<Comment>>, NotificationManager<ReportSubmission<Comment>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Article>>, ArticleReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Article>>, ArticleReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<Comment>>, CommentReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Comment>>, CommentReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Article>, ReportArticleManager>();
            services.AddScoped<IReportEntityManager<Comment>, ReportCommentManager>();
            
            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Article>, FederatedQueryManager<Article>>();
            services.AddScoped<IFederatedQueryProvider<Article>, EntityQueries<Article>>();
            
            // Query adapters
            services.AddScoped<IQueryAdapterManager<Article>, QueryAdapterManager<Article>>();
         
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
                name: "Articles",
                areaName: "Plato.Articles",
                template: "articles/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Popular
            routes.MapAreaRoute(
                name: "ArticlesPopular",
                areaName: "Plato.Articles",
                template: "articles/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "ArticlesArticle",
                areaName: "Plato.Articles",
                template: "articles/a/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "ArticleNew",
                areaName: "Plato.Articles",
                template: "articles/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // Edit Entity
            routes.MapAreaRoute(
                name: "ArticlesEdit",
                areaName: "Plato.Articles",
                template: "articles/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "ArticlesDisplayReply",
                areaName: "Plato.Articles",
                template: "articles/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "ArticlesReport",
                areaName: "Plato.Articles",
                template: "articles/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );
            
            // User Index
            routes.MapAreaRoute(
                name: "ArticlesUser",
                areaName: "Plato.Articles",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/articles/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

        }
    }
}