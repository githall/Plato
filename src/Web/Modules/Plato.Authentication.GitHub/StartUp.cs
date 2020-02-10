using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OAuth.GitHub;
using Plato.Authentication.GitHub.Configuration;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using Plato.Authentication.GitHub.ViewProviders;

namespace Plato.Authentication.GitHub
{
    public class Startup : StartupBase
    {

        // Uses the great AspNet.Security.OAuth.Providers project @
        // https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers

        public override void ConfigureServices(IServiceCollection services)
        {

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