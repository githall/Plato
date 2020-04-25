using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using Plato.Questions.Share.Navigation;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Questions.Share.Handlers;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Questions.Share
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

            // Navigation providers
            services.AddScoped<INavigationProvider, QuestionMenu>();
            services.AddScoped<INavigationProvider, QuestionAnswerMenu>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "QuestionsShare",
                areaName: "Plato.Questions.Share",
                template: "questions/q/share/{opts.id}/{opts.alias}/{opts.replyId?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
        }

    }

}