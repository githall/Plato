using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Twitter.Models;
using Plato.Twitter.Stores;

namespace Plato.Twitter.Configuration
{
    public class PlatoTwitterOptionsConfiguration : IConfigureOptions<PlatoTwitterOptions>
    {

        private readonly ITwitterSettingsStore<PlatoTwitterSettings> _TwitterSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<PlatoTwitterOptionsConfiguration> _logger;

        public PlatoTwitterOptionsConfiguration(
            ITwitterSettingsStore<PlatoTwitterSettings> TwitterSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<PlatoTwitterOptionsConfiguration> logger)
        {
            _TwitterSettingsStore = TwitterSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoTwitterOptionsConfiguration));
                        options.ConsumerSecret = protector.Unprotect(settings.ConsumerSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the twitter consumer key secret. {e.Message}");
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoTwitterOptionsConfiguration));
                        options.AccessTokenSecret = protector.Unprotect(settings.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the twitter access token secret. {e.Message}");
                    }
                }
                
            }

        }

    }
}
