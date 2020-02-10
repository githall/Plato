using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Badges;
using Plato.Discuss.Tags.Handlers;
using Plato.Discuss.Tags.Models;
using Plato.Discuss.Tags.Navigation;
using Plato.Discuss.Tags.Tasks;
using Plato.Discuss.Tags.ViewAdapters;
using Plato.Discuss.Tags.ViewProviders;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Tags.Repositories;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.Subscribers;
using Plato.Entities.Tags.Subscribers;

namespace Plato.Discuss.Tags
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
            services.AddScoped<INavigationProvider, TopicFooterMenu>();
            services.AddScoped<INavigationProvider, TopicReplyFooterMenu>();
            
            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();
        
            // Admin view providers
            services.AddScoped<IViewProviderManager<TagAdmin>, ViewProviderManager<TagAdmin>>();
            services.AddScoped<IViewProvider<TagAdmin>, AdminViewProvider>();

            // Tag view providers
            services.AddScoped<IViewProviderManager<Tag>, ViewProviderManager<Tag>>();
            services.AddScoped<IViewProvider<Tag>, TagViewProvider>();
         
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, TopicListItemViewAdapter>();
            
            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TagBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TagBadgesAwarder>();

            // Notification manager
            services.AddScoped<INotificationManager<Badge>, NotificationManager<Badge>>();

            // Data access
            services.AddScoped<ITagOccurrencesUpdater<Tag>, TagOccurrencesUpdater<Tag>>();
            services.AddScoped<ITagRepository<Tag>, TagRepository<Tag>>();
            services.AddScoped<ITagService<Tag>, TagService<Tag>>();
            services.AddScoped<ITagManager<Tag>, TagManager<Tag>>();
            services.AddScoped<ITagStore<Tag>, TagStore<Tag>>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
        
            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityTagSubscriber<Tag>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Tag Index
            routes.MapAreaRoute(
                name: "DiscussTagIndex",
                areaName: "Plato.Discuss.Tags",
                template: "discuss/tags/{pager.offset:int?}/{opts.search?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Tag Entities
            routes.MapAreaRoute(
                name: "DiscussTagDisplay",
                areaName: "Plato.Discuss.Tags",
                template: "discuss/tag/{opts.tagId:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );
            
        }

    }

}