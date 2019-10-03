using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.Stores;

namespace Plato.Site.Demo.Configuration
{

    public class DemoOptionsConfiguration : IConfigureOptions<DemoOptions>
    {

        private readonly IDemoSettingsStore<DemoSettings> _TwitterSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<DemoOptionsConfiguration> _logger;

        public DemoOptionsConfiguration(
            IDemoSettingsStore<DemoSettings> TwitterSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<DemoOptionsConfiguration> logger)
        {
            _TwitterSettingsStore = TwitterSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(DemoOptions options)
        {

            var settings = _TwitterSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                options.DemoEnabled = settings.DemoEnabled;

                // ------------------
                // Default administrator account
                // ------------------

                options.AdminUserName = settings.AdminUserName;

                // Decrypt the password
                if (!String.IsNullOrWhiteSpace(settings.AdminPassword))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(DemoOptionsConfiguration));
                        options.AdminPassword = protector.Unprotect(settings.AdminPassword);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the twitter consumer key secret. {e.Message}");
                    }
                }

            }

        }

    }

}
