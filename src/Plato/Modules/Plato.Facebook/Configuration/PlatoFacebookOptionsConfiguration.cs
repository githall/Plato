using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;

namespace Plato.Facebook.Configuration
{
    public class PlatoFacebookOptionsConfiguration : IConfigureOptions<PlatoFacebookOptions>
    {

        private readonly IFacebookSettingsStore<PlatoFacebookSettings> _facebookSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<PlatoFacebookOptionsConfiguration> _logger;

        public PlatoFacebookOptionsConfiguration(
            IFacebookSettingsStore<PlatoFacebookSettings> facebookSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<PlatoFacebookOptionsConfiguration> logger)
        {
            _facebookSettingsStore = facebookSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoFacebookOptionsConfiguration));
                        options.AppSecret = protector.Unprotect(settings.AppSecret);
                    }
                    catch
                    {
                        _logger.LogError("There was a problem decrypting the SMTP password.");
                    }
                }

            }

        }

    }

}
