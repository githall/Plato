﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.Models;
using Plato.Ideas.Tags.Badges;
using Plato.Ideas.Tags.Handlers;
using Plato.Ideas.Tags.Models;
using Plato.Ideas.Tags.Navigation;
using Plato.Ideas.Tags.Tasks;
using Plato.Ideas.Tags.ViewAdapters;
using Plato.Ideas.Tags.ViewProviders;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
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
using Plato.Tags.Subscribers;
using Plato.Entities.Tags.Subscribers;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Ideas.Tags
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
            services.AddScoped<INavigationProvider, IdeaFooterMenu>();
            services.AddScoped<INavigationProvider, IdeaCommentFooterMenu>();
            
            // Discuss view providers
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();
            services.AddScoped<IViewProviderManager<IdeaComment>, ViewProviderManager<IdeaComment>>();
            services.AddScoped<IViewProvider<IdeaComment>, IdeaCommentViewProvider>();
        
            // Admin view providers
            services.AddScoped<IViewProviderManager<TagAdmin>, ViewProviderManager<TagAdmin>>();
            services.AddScoped<IViewProvider<TagAdmin>, AdminViewProvider>();

            // Tag view providers
            services.AddScoped<IViewProviderManager<Tag>, ViewProviderManager<Tag>>();
            services.AddScoped<IViewProvider<Tag>, TagViewProvider>();
         
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, IdeaListItemViewAdapter>();

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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Idea, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<IdeaComment, Tag>>();
            services.AddScoped<IBrokerSubscriber, EntityTagSubscriber<Tag>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Tag Index
            routes.MapAreaRoute(
                name: "IdeasTagIndex",
                areaName: "Plato.Ideas.Tags",
                template: "ideas/tags/{pager.offset:int?}/{opts.search?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Tag Entities
            routes.MapAreaRoute(
                name: "IdeasTagDisplay",
                areaName: "Plato.Ideas.Tags",
                template: "ideas/tag/{opts.tagId:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

        }

    }

}