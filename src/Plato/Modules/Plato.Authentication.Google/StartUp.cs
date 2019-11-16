using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Authentication.Google.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Plato.Authentication.Google.Configuration;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Authentication.Google.ViewProviders;

namespace Plato.Authentication.Google
{
    public class Startup : StartupBase
    {

        private readonly GoogleOptions _googleOptions;

        public Startup(IOptions<GoogleOptions> googleOptions)
        {
            _googleOptions = googleOptions.Value;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Configuration
            services.AddTransient<IConfigureOptions<AuthenticationOptions>, GoogleSchemaConfiguration>();
            services.AddTransient<IConfigureOptions<GoogleOptions>, GoogleSchemaConfiguration>();

            // Built-in initializers:
            services.AddTransient<IPostConfigureOptions<GoogleOptions>, OAuthPostConfigureOptions<GoogleOptions, GoogleHandler>>();

            // Login view provider
            services.AddScoped<IViewProviderManager<UserLogin>, ViewProviderManager<UserLogin>>();
            services.AddScoped<IViewProvider<UserLogin>, LoginViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}