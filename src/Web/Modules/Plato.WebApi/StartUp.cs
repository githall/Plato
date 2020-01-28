using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Text.Abstractions;
using Plato.WebApi.Middleware;
using Plato.WebApi.Models;
using Plato.WebApi.Navigation;
using Plato.WebApi.Services;
using Plato.WebApi.ViewProviders;

namespace Plato.WebApi
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

            // Rewrite web api configuration
            // Removed as we require asynchronous calls and wanted to avoid GetAwaiter()
            // Using IWebApiSettingsFactory instead
            //services.TryAddEnumerable(ServiceDescriptor
            //    .Transient<IConfigureOptions<WebApiOptions>, WebApiOptionsConfiguration>());

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers (adds API key to admin / edit user)
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<WebApiSettings>, ViewProviderManager<WebApiSettings>>();
            services.AddScoped<IViewProvider<WebApiSettings>, AdminViewProvider>();

            // Services
            services.AddScoped<IWebApiAuthenticator, WebApiAuthenticator>();
            services.AddScoped<IWebApiOptionsFactory, WebApiOptionsFactory>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Register client options middleware 
            app.UseMiddleware<WebApiClientOptionsMiddleware>();

            // Generate CSRF token for client api requests
            var keyGenerator = app.ApplicationServices.GetService<IKeyGenerator>();
            var csrfToken = keyGenerator.GenerateKey(o => { o.MaxLength = 75; });

            // Add client accessible CSRF token for web api requests
            app.Use(next => ctx =>
            {
                // ensure the cookie does not already exist
                var cookie = ctx.Request.Cookies[PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName];
                if (cookie == null)
                {
                    ctx.Response.Cookies.Append(PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName, csrfToken,
                        new CookieOptions() {HttpOnly = false});
                }
                else
                {
                    // Delete any existing cookie
                    ctx.Response.Cookies.Delete(cookie);
                    // Create new cookie
                    ctx.Response.Cookies.Append(PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName, csrfToken,
                        new CookieOptions() {HttpOnly = false});
                }
                return next(ctx);
            });
            
            // WebApi Settings
            routes.MapAreaRoute(
                name: "WebApiAdmin",
                areaName: "Plato.WebApi",
                template: "admin/settings/api",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Reset API Key
            routes.MapAreaRoute(
                name: "ResetApiKey",
                areaName: "Plato.WebApi",
                template: "admin/settings/api/reset",
                defaults: new { controller = "Admin", action = "ResetApiKey" }
            );
            
            // Reset user API Key
            routes.MapAreaRoute(
                name: "ResetUserApiKey",
                areaName: "Plato.WebApi",
                template: "admin/users/{id}/api/reset",
                defaults: new { controller = "Admin", action = "ResetUserApiKey" }
            );

            // Api routes
            routes.MapAreaRoute(
                name: "PlatoWebApi",
                areaName: "Plato.WebApi",
                template: "api/{controller}/{action}/{id?}",
                defaults: new { controller = "Users", action = "Get" }
            );

        }
        
    }

}