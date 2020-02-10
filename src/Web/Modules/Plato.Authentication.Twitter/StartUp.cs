using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Plato.Authentication.Twitter.Configuration;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using Plato.Authentication.Twitter.ViewProviders;

namespace Plato.Authentication.Twitter
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Configuration
            services.AddTransient<IConfigureOptions<AuthenticationOptions>, TwitterSchemeConfiguration>();
            services.AddTransient<IConfigureOptions<TwitterOptions>, TwitterSchemeConfiguration>();

            // Built-in initializers:
            services.AddTransient<IPostConfigureOptions<TwitterOptions>, TwitterPostConfigureOptions>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

        }

    }

}