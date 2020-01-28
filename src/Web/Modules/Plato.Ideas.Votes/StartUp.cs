using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Ratings.Services;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Ideas.Models;
using Plato.Ideas.Votes.Handlers;
using Plato.Ideas.Votes.Navigation;

namespace Plato.Ideas.Votes
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
            services.AddScoped<INavigationProvider, IdeaDetailsMenu>();
            services.AddScoped<INavigationProvider, IdeaCommentDetailsMenu>();

            // Entity rating aggregator
            services.AddScoped<IEntityRatingAggregator<Idea>, EntityRatingAggregator<Idea>>();
            services.AddScoped<IEntityReplyRatingAggregator<IdeaComment>, EntityReplyRatingAggregator<IdeaComment>>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "IdeaVotesWebApi",
                areaName: "Plato.Ideas.Votes",
                template: "api/ideas/{controller}/{action}/{id?}",
                defaults: new { controller = "Vote", action = "Get" }
            );


        }
    }
}