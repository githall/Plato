using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using PlatoCore.Models.Users;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Authentication.Facebook.Configuration;
using Plato.Authentication.Facebook.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Authentication.Facebook
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

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