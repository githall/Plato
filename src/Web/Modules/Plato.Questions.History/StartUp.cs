﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.History.Navigation;
using Plato.Questions.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using Plato.Entities.History.Subscribers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Questions.History.Handlers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Questions.History
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
            services.AddScoped<INavigationProvider, QuestionMenu>();
            services.AddScoped<INavigationProvider, AnswerMenu>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Question>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Answer>>();
          
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "QuestionHistory",
                areaName: "Plato.Questions.History",
                template: "questions/history/{id:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "QuestionRollbackHistory",
                areaName: "Plato.Questions.History",
                template: "questions/history/rollback",
                defaults: new { controller = "Home", action = "Rollback" }
            );

            routes.MapAreaRoute(
                name: "QuestionDeleteHistory",
                areaName: "Plato.Questions.History",
                template: "questions/history/delete",
                defaults: new { controller = "Home", action = "Delete" }
            );

        }

    }

}