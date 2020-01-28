using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Google.Stores;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Google.Configuration
{
    public class PlatoGoogleOptionsConfiguration : IConfigureOptions<PlatoGoogleOptions>
    {

        private readonly IGoogleSettingsStore<PlatoGoogleSettings> _googleSettingsStore;        
        private readonly ILogger<PlatoGoogleOptionsConfiguration> _logger;        
        private readonly IEncrypter _encrypter;

        public PlatoGoogleOptionsConfiguration(
            IGoogleSettingsStore<PlatoGoogleSettings> googleSettingsStore,            
            ILogger<PlatoGoogleOptionsConfiguration> logger,            
            IEncrypter encrypter)
        {            
            _googleSettingsStore = googleSettingsStore;
            _encrypter = encrypter;
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
                        options.ClientSecret = _encrypter.Decrypt(settings.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Google client secret. {e.Message}");
                        }
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
