﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Ratings.Services;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Questions.Models;
using Plato.Questions.Votes.Handlers;
using Plato.Questions.Votes.Navigation;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Questions.Votes
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
            services.AddScoped<INavigationProvider, QuestionDetailsMenu>();
            services.AddScoped<INavigationProvider, QuestionAnswerDetailsMenu>();

            // Entity rating aggregator
            services.AddScoped<IEntityRatingAggregator<Question>, EntityRatingAggregator<Question>>();
            services.AddScoped<IEntityReplyRatingAggregator<Answer>, EntityReplyRatingAggregator<Answer>>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "QuestionVotesWebApi",
                areaName: "Plato.Questions.Votes",
                template: "api/questions/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }

    }

}