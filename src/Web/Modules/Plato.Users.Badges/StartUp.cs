using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Users.Badges.Navigation;
using Plato.Users.Badges.Notifications;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Badges.NotificationTypes;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using Plato.Users.Badges.Handlers;
using Plato.Users.Badges.Services;
using Plato.Users.Badges.Tasks;
using Plato.Users.Badges.ViewProviders;

namespace Plato.Users.Badges
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

            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, VisitBadges>();
            services.AddScoped<IBadgesProvider<Badge>, ProfileBadges>();
            services.AddScoped<IBadgesProvider<Badge>, ReputationBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, AutobiographerBadgeAwarder>();
            services.AddScoped<IBackgroundTaskProvider, PersonalizerBadgeAwarder>();
            services.AddScoped<IBackgroundTaskProvider, ConfirmedMemberBadgeAwarder>();
            services.AddScoped<IBackgroundTaskProvider, VisitBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, ReputationBadgesAwarder>();

            // User profile view providers
            services.AddScoped<IViewProviderManager<ProfilePage>, ViewProviderManager<ProfilePage>>();
            services.AddScoped<IViewProvider<ProfilePage>, ProfileViewProvider>();

            // Services
            services.AddScoped<IBadgeEntriesStore, BadgeEntriesStore>();

            // Badge view providers
            services.AddScoped<IViewProviderManager<Badge>, ViewProviderManager<Badge>>();
            services.AddScoped<IViewProvider<Badge>, BadgeViewProvider>();

            // User badges view providers
            services.AddScoped<IViewProviderManager<UserBadge>, ViewProviderManager<UserBadge>>();
            services.AddScoped<IViewProvider<UserBadge>, UserBadgeViewProvider>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, ProfileMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            
            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Badge>, NotificationManager<Badge>>();
            
            // Notification Providers
            services.AddScoped<INotificationProvider<Badge>, NewBadgeEmail>();
            services.AddScoped<INotificationProvider<Badge>, NewBadgeWeb>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "BadgesIndex",
                areaName: "Plato.Users.Badges",
                template: "b/{offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DisplayUserBadges",
                areaName: "Plato.Users.Badges",
                template: "u/{opts.id:int}/{opts.alias?}/badges",
                defaults: new { controller = "Profile", action = "Index" }
            );

        }
    }
}