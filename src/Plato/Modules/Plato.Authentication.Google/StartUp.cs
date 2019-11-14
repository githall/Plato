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
using Microsoft.AspNetCore.Identity;
using Plato.Authentication.Google.Configuration;

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

           // // Add & configure Google authentication via Microsoft.AspNetCore.Authentication.Google
           // services.AddAuthentication(options =>
           // {
           //     options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
           //     options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
           //     options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
           // }).AddGoogle(options =>
           //{
           //    options.ClientId = _googleOptions.ClientId;
           //    options.ClientSecret = _googleOptions.ClientSecret;
           //});

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}