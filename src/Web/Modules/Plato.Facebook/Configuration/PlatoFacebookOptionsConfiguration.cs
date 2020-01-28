using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Facebook.Configuration
{
    public class PlatoFacebookOptionsConfiguration : IConfigureOptions<PlatoFacebookOptions>
    {

        private readonly IFacebookSettingsStore<PlatoFacebookSettings> _facebookSettingsStore;        
        private readonly ILogger<PlatoFacebookOptionsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public PlatoFacebookOptionsConfiguration(
            IFacebookSettingsStore<PlatoFacebookSettings> facebookSettingsStore,            
            ILogger<PlatoFacebookOptionsConfiguration> logger,
            IEncrypter encrypter)
        {
            _facebookSettingsStore = facebookSettingsStore;            
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(PlatoFacebookOptions options)
        {

            var settings = _facebookSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.AppId = settings.AppId;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.AppSecret))
                {
                    try
                    {                        
                        options.AppSecret = _encrypter.Decrypt(settings.AppSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Facebook app secret. {e.Message}");
                        }                        
                    }
                }

            }

        }

    }

}
