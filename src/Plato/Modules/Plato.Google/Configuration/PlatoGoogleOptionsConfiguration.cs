using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Google.Stores;

namespace Plato.Google.Configuration
{
    public class PlatoGoogleOptionsConfiguration : IConfigureOptions<PlatoGoogleOptions>
    {

        private readonly IGoogleSettingsStore<PlatoGoogleSettings> _googleSettingsStore;        
        private readonly ILogger<PlatoGoogleOptionsConfiguration> _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public PlatoGoogleOptionsConfiguration(
            IGoogleSettingsStore<PlatoGoogleSettings> googleSettingsStore,            
            ILogger<PlatoGoogleOptionsConfiguration> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider; 
            _googleSettingsStore = googleSettingsStore;            
            _logger = logger;
        }

        public void Configure(PlatoGoogleOptions options)
        {

            var settings = _googleSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                // Authentication

                options.ClientId = settings.ClientId;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoGoogleOptionsConfiguration));
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

                // Analytics

                options.TrackingId = settings.TrackingId;

            }

        }

    }

}
