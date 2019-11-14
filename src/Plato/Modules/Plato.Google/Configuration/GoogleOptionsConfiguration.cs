using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Google.Stores;

namespace Plato.Google.Configuration
{
    public class GoogleOptionsConfiguration : IConfigureOptions<GoogleOptions>
    {

        private readonly IGoogleSettingsStore<GoogleSettings> _googleSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<GoogleOptionsConfiguration> _logger;

        public GoogleOptionsConfiguration(
            IGoogleSettingsStore<GoogleSettings> googleSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<GoogleOptionsConfiguration> logger)
        {
            _googleSettingsStore = googleSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(GoogleOptions options)
        {

            var settings = _googleSettingsStore
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(GoogleOptionsConfiguration));
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
