using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using PlatoCore.Navigation;
using PlatoCore.Navigation.Abstractions;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Services;
using Plato.Users.reCAPTCHA2.Navigation;
using Plato.Users.reCAPTCHA2.Stores;
using Plato.Users.reCAPTCHA2.ViewProviders;

namespace Plato.Users.reCAPTCHA2
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
            // Stores
            services.AddScoped<IReCaptchaSettingsStore<ReCaptchaSettings>, ReCaptchaSettingsStore>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();
            
            // Admin view provider
            services.AddScoped<IViewProviderManager<ReCaptchaSettings>, ViewProviderManager<ReCaptchaSettings>>();
            services.AddScoped<IViewProvider<ReCaptchaSettings>, AdminViewProvider>();
            
            // Register services
            services.AddScoped<IReCaptchaService, ReCaptchaService>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoReCAPTCHA2Admin",
                areaName: "Plato.Users.reCAPTCHA2",
                template: "admin/settings/recaptcha2",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}