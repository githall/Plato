﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Issues.Handlers;
using Plato.Issues.Assets;
using Plato.Issues.Badges;
using Plato.Issues.Models;
using Plato.Issues.Navigation;
using Plato.Issues.Notifications;
using Plato.Issues.NotificationTypes;
using Plato.Issues.Services;
using Plato.Issues.Subscribers;
using Plato.Issues.Tasks;
using Plato.Issues.ViewProviders;
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
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues
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
            services.AddScoped<INavigationProvider, IssueMenu>();
            services.AddScoped<INavigationProvider, IssueCommentMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Issue>, EntityRepository<Issue>>();
            services.AddScoped<IEntityStore<Issue>, EntityStore<Issue>>();
            services.AddScoped<IEntityManager<Issue>, EntityManager<Issue>>();

            services.AddScoped<IEntityReplyRepository<Comment>, EntityReplyRepository<Comment>>();
            services.AddScoped<IEntityReplyStore<Comment>, EntityReplyStore<Comment>>();
            services.AddScoped<IEntityReplyManager<Comment>, EntityReplyManager<Comment>>();

            //  Post managers
            services.AddScoped<IPostManager<Issue>, IssueManager>();
            services.AddScoped<IPostManager<Comment>, ReplyManager>();

            // Entity services - transient as they contains action
            // delegates that can change state several times per request
            services.AddTransient<IEntityService<Issue>, EntityService<Issue>>();
            services.AddTransient<IEntityReplyService<Comment>, EntityReplyService<Comment>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Issue>, EntityViewIncrementer<Issue>>();

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
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();

            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, ArticleSubscriber<Issue>>();
            services.AddScoped<IBrokerSubscriber, IssueCommentSubscriber<Comment>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, IssueBadges>();
            services.AddScoped<IBadgesProvider<Badge>, IssueCommentBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, IssueBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, CommentBadgesAwarder>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Issue>>, NotificationManager<ReportSubmission<Issue>>>();
            services.AddScoped<INotificationManager<ReportSubmission<Comment>>, NotificationManager<ReportSubmission<Comment>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Issue>>, IssueReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Issue>>, IssueReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<Comment>>, CommentReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Comment>>, CommentReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Issue>, ReportArticleManager>();
            services.AddScoped<IReportEntityManager<Comment>, ReportCommentManager>();
            
            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Issue>, FederatedQueryManager<Issue>>();
            services.AddScoped<IFederatedQueryProvider<Issue>, EntityQueries<Issue>>();
        
            // Query adapters
            services.AddScoped<IQueryAdapterManager<Issue>, QueryAdapterManager<Issue>>();
            
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
                name: "Issues",
                areaName: "Plato.Issues",
                template: "issues/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Popular
            routes.MapAreaRoute(
                name: "IssuesPopular",
                areaName: "Plato.Issues",
                template: "issues/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "IssuesDisplay",
                areaName: "Plato.Issues",
                template: "issues/i/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "IssuesNew",
                areaName: "Plato.Issues",
                template: "issues/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // Edit Entity
            routes.MapAreaRoute(
                name: "IssuesEdit",
                areaName: "Plato.Issues",
                template: "issues/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "IssuesDisplayReply",
                areaName: "Plato.Issues",
                template: "issues/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "IssuesReport",
                areaName: "Plato.Issues",
                template: "issues/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );
            
            // User Index
            routes.MapAreaRoute(
                name: "IssuesUser",
                areaName: "Plato.Issues",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/issues/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

            // Admin Index
            routes.MapAreaRoute(
                name: "IssuesAdminIndex",
                areaName: "Plato.Issues",
                template: "admin/issues",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}