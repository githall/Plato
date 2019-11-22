using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Security.Abstractions.Encryption;
using Plato.Internal.Security.Attributes;
using Plato.Internal.Security.Configuration;
using Plato.Internal.Security.Encryption;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoSecurity(
            this IServiceCollection services)
        {
            services.AddPlatoAuthentication(); 
            services.AddPlatoAuthorization();
            services.AddPlatoEncryption();
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
            services.AddAuthentication();           
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

        public static IServiceCollection AddPlatoEncryption(this IServiceCollection services)
        {

            // Private key store
            services.AddSingleton<IPlatoKeyStore, PlatoKeyStore>();

            // Configuration
            services.AddSingleton<IConfigureOptions<PlatoKeyOptions>, PlatoKeyOptionsConfiguration>();

            // Default encryption
            services.AddTransient<IAesEncrypter, AesEncrypter>();
            services.AddTransient<IEncrypter, DefaultEncrypter>();

            return services;

        }

    }

}
