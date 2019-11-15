using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Security.Attributes;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoSecurity(
            this IServiceCollection services)
        {
            services.AddPlatoAuthorization();
            services.AddPlatoAuthentication();
            return services;
        }

        public static IServiceCollection AddPlatoAuthorization(this IServiceCollection services)
        {

            // Permissions manager
            services.AddScoped<IPermissionsManager<Permission>, PermissionsManager<Permission>>();

            // Add core authorization services 
            services.AddAuthorization();

            return services;

        }

        public static IServiceCollection AddPlatoAuthentication(this IServiceCollection services)
        {

            // Configure authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                // Configure the authentication options to use the application cookie scheme as the default sign-out handler.
                // This is required for security modules like the OpenID module (that uses SignOutAsync()) to work correctly.
                options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;

            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/login");
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        {
                            await SecurityStampValidator.ValidatePrincipalAsync(context);
                        }
                    };
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                })
                .AddCookie(IdentityConstants.TwoFactorUserIdScheme, IdentityConstants.TwoFactorUserIdScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });

            return services;

        }

        public static IServiceCollection AddPlatoDataProtection(
            this IServiceCollection services)
        {

            // Attempt to get secrets path from appsettings.json file
            // If found register file system storage of private keys
            var opts = services.BuildServiceProvider().GetService<IOptions<PlatoOptions>>();
            if (opts != null)
            {
                if (!string.IsNullOrEmpty(opts.Value.SecretsPath))
                {
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(opts.Value.SecretsPath));
                }
            }

            return services;

        }

        public static IServiceCollection AddPlatoModelValidation(
            this IServiceCollection services)
        {
            // Custom validation providers
            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidatiomAttributeAdapterProvider>();
            return services;
        }

    }

}
