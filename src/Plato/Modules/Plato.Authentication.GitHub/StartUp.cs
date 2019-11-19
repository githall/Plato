using Microsoft.Extensions.DependencyInjection;
using Plato.Authentication.GitHub.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Plato.Authentication.GitHub.Configuration;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Authentication.GitHub.ViewProviders;
using AspNet.Security.OAuth.GitHub;

namespace Plato.Authentication.GitHub
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Configuration
            services.AddTransient<IConfigureOptions<AuthenticationOptions>, GitHubSchemeConfiguration>();
            services.AddTransient<IConfigureOptions<GitHubAuthenticationOptions>, GitHubSchemeConfiguration>();

            // Built-in initializers:
            services.AddTransient<IPostConfigureOptions<GitHubAuthenticationOptions>, OAuthPostConfigureOptions<GitHubAuthenticationOptions, GitHubAuthenticationHandler>>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

        }

    }

}