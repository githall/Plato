using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Articles.Models;
using Plato.Articles.Tags.Badges;
using Plato.Articles.Tags.Handlers;
using Plato.Articles.Tags.Models;
using Plato.Articles.Tags.Navigation;
using Plato.Articles.Tags.Tasks;
using Plato.Articles.Tags.ViewAdapters;
using Plato.Articles.Tags.ViewProviders;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Tags.Repositories;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Entities.Tags.Subscribers;
using PlatoCore.Messaging.Abstractions;
using Plato.Tags.Subscribers;

namespace Plato.Articles.Tags
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
            services.AddScoped<INavigationProvider, ArticleFooterMenu>();
            services.AddScoped<INavigationProvider, ArticleCommentFooterMenu>();
            
            // Discuss view providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();
        
            // Admin view providers
            services.AddScoped<IViewProviderManager<TagAdmin>, ViewProviderManager<TagAdmin>>();
            services.AddScoped<IViewProvider<TagAdmin>, AdminViewProvider>();

            // Tag view providers
            services.AddScoped<IViewProviderManager<Tag>, ViewProviderManager<Tag>>();
            services.AddScoped<IViewProvider<Tag>, TagViewProvider>();
         
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, ArticleListItemViewAdapter>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TagBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TagBadgesAwarder>();

            // Notification manager
            services.AddScoped<INotificationManager<Badge>, NotificationManager<Badge>>();

            // Data access
            services.AddScoped<ITagOccurrencesUpdater<Tag>, TagOccurrencesUpdater<Tag>>();
            services.AddScoped<ITagRepository<Tag>, TagRepository<Tag>>();
            services.AddScoped<ITagStore<Tag>, TagStore<Tag>>();
            services.AddScoped<ITagService<Tag>, TagService<Tag>>();
            services.AddScoped<ITagManager<Tag>, TagManager<Tag>>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
          
            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Article, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityTagSubscriber<Tag>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Tag Index
            routes.MapAreaRoute(
                name: "ArticlesTagIndex",
                areaName: "Plato.Articles.Tags",
                template: "articles/tags/{pager.offset:int?}/{opts.search?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Tag Entities
            routes.MapAreaRoute(
                name: "ArticlesTagDisplay",
                areaName: "Plato.Articles.Tags",
                template: "articles/tag/{opts.tagId:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );
            
        }

    }

}