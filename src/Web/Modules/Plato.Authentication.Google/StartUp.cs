using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Plato.Authentication.Google.Configuration;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using Plato.Authentication.Google.ViewProviders;

namespace Plato.Authentication.Google
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Configuration
            services.AddTransient<IConfigureOptions<AuthenticationOptions>, GoogleSchemeConfiguration>();
            services.AddTransient<IConfigureOptions<GoogleOptions>, GoogleSchemeConfiguration>();

            // Built-in initializers:
            services.AddTransient<IPostConfigureOptions<GoogleOptions>, OAuthPostConfigureOptions<GoogleOptions, GoogleHandler>>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

        }

    }

}