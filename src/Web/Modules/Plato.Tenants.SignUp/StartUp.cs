using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.SetUp;
using Plato.Tenants.SignUp.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Tenants.SignUp.Stores;
using Plato.Tenants.SignUp.Services;
using Plato.Tenants.SignUp.Repositories;
using PlatoCore.Abstractions.Routing;

namespace Plato.Tenants.SignUp
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

            // Repositories
            services.AddScoped<ISignUpRepository<Models.SignUp>, SignUpRepository>();

            // Stores            
            services.AddScoped<ISignUpStore<Models.SignUp>, SignUpStore>();

            // Services
            services.AddScoped<ISignUpManager<Models.SignUp>, SignUpManager>();
            services.AddScoped<ISignUpValidator, SignUpValidator>();
            services.AddScoped<ISignUpEmailService, SignUpEmailService>();

            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // SignUp
            routes.MapAreaRoute(
                name: "PlatoSiteSignUp",
                areaName: "Plato.Tenants.SignUp",
                template: "try",
                defaults: new { controller = "Home", action = "Index" }
            );

            // SignUp Confirmation
            routes.MapAreaRoute(
                name: "PlatoSiteSignUpConfirmation",
                areaName: "Plato.Tenants.SignUp",
                template: "try/confirm/{sessionId}",
                defaults: new { controller = "Home", action = "IndexConfirmation" }
            );

            // SetUp
            routes.MapAreaRoute(
                name: "PlatoSiteSetUp",
                areaName: "Plato.Tenants.SignUp",
                template: "try/company/{sessionId}",
                defaults: new { controller = "Home", action = "SetUp" }
            );

            // SetUpConfirmation
            routes.MapAreaRoute(
                name: "PlatoSiteSetUpConfirmation",
                areaName: "Plato.Tenants.SignUp",
                template: "try/account/{sessionId}",
                defaults: new { controller = "Home", action = "SetUp" }
            );

            // SetUp Complete
            routes.MapAreaRoute(
                name: "PlatoSiteSetUpComplete",
                areaName: "Plato.Tenants.SignUp",
                template: "try/complete/{sessionId}",
                defaults: new { controller = "Home", action = "SetUpComplete" }
            );


        }

    }

}