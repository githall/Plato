using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Google.Stores;

namespace Plato.Google.Configuration
{
    public class GoogleAuthenticationOptionsConfiguration : IConfigureOptions<GoogleAuthenticationOptions>
    {

        private readonly IGoogleSettingsStore<GoogleSettings> _googleSettingsStore;        
        private readonly ILogger<GoogleAuthenticationOptionsConfiguration> _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public GoogleAuthenticationOptionsConfiguration(
            IGoogleSettingsStore<GoogleSettings> googleSettingsStore,            
            ILogger<GoogleAuthenticationOptionsConfiguration> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider; 
            _googleSettingsStore = googleSettingsStore;            
            _logger = logger;
        }

        public void Configure(GoogleAuthenticationOptions options)
        {

            var settings = _googleSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.ClientId = settings.ClientId;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(GoogleAuthenticationOptionsConfiguration));
                        options.ClientSecret = protector.Unprotect(settings.ClientSecret);
                    }
                    catch
                    {
                        _logger.LogError("There was a problem decrypting the SMTP password.");
                    }
                }

                if (settings.CallbackPath.HasValue)
                {
                    options.CallbackPath = settings.CallbackPath;
                }

            }

        }

    }

}
