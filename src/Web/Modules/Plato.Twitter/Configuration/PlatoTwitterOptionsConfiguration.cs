using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Twitter.Models;
using Plato.Twitter.Stores;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Twitter.Configuration
{

    public class PlatoTwitterOptionsConfiguration : IConfigureOptions<PlatoTwitterOptions>
    {

        private readonly ITwitterSettingsStore<PlatoTwitterSettings> _TwitterSettingsStore;        
        private readonly ILogger<PlatoTwitterOptionsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public PlatoTwitterOptionsConfiguration(
            ITwitterSettingsStore<PlatoTwitterSettings> TwitterSettingsStore,
            ILogger<PlatoTwitterOptionsConfiguration> logger,
            IEncrypter encrypter)
        {
            _TwitterSettingsStore = TwitterSettingsStore;            
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(PlatoTwitterOptions options)
        {

            var settings = _TwitterSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                // ------------------
                // Consumer Keys
                // ------------------

                options.ConsumerKey = settings.ConsumerKey;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.ConsumerSecret))
                {
                    try
                    {
                        options.ConsumerSecret = _encrypter.Decrypt(settings.ConsumerSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Twitter consumer key secret. {e.Message}");
                        }
                    }
                }

                // ------------------
                // Access Tokens
                // ------------------

                options.AccessToken = settings.AccessToken;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.AccessTokenSecret))
                {
                    try
                    {                        
                        options.AccessTokenSecret = _encrypter.Decrypt(settings.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Twitter access token secret. {e.Message}");
                        }
                    }
                }
                
            }

        }

    }
}
