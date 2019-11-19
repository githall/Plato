using Microsoft.Extensions.DependencyInjection;
using Plato.Authentication.Facebook.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Plato.Authentication.Facebook.Configuration;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Authentication.Facebook.ViewProviders;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Plato.Authentication.Facebook
{
    public class Startup : StartupBase
    {

        // Uses the great AspNet.Security.OAuth.Providers project @
        // https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Configuration
            services.AddTransient<IConfigureOptions<AuthenticationOptions>, FacebookSchemeConfiguration>();
            services.AddTransient<IConfigureOptions<FacebookOptions>, FacebookSchemeConfiguration>();

            // Built-in initializers:
            services.AddTransient<IPostConfigureOptions<FacebookOptions>, OAuthPostConfigureOptions<FacebookOptions, FacebookHandler>>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

        }

    }

}