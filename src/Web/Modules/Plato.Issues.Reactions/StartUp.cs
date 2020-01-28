using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Reactions.Handlers;
using Plato.Issues.Reactions.Navigation;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Issues.Reactions.Badges;
using Plato.Issues.Reactions.Tasks;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;

namespace Plato.Issues.Reactions
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
            services.AddScoped<INavigationProvider, IssueMenu>();
            services.AddScoped<INavigationProvider, IssueFooterMenu>();
            services.AddScoped<INavigationProvider, IssueCommentMenu>();
            services.AddScoped<INavigationProvider, IdeaCommentFooterMenu>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
                  
            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ReactionBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, ReactionBadgesAwarder>();
          
            // Reaction providers
            services.AddScoped<IReactionsProvider<Reaction>, Reactions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}