using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.Stores;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Site.Demo.Configuration
{

    public class DemoOptionsConfiguration : IConfigureOptions<DemoOptions>
    {

        private readonly IDemoSettingsStore<DemoSettings> _demoSettingsStore;        
        private readonly ILogger<DemoOptionsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public DemoOptionsConfiguration(
            IDemoSettingsStore<DemoSettings> demoSettingsStore,            
            ILogger<DemoOptionsConfiguration> logger,
            IEncrypter encrypter)
        {
            _demoSettingsStore = demoSettingsStore;            
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(DemoOptions options)
        {

            var settings = _demoSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                // ------------------
                // Default administrator account
                // ------------------

                options.AdminUserName = settings.AdminUserName;

                // Decrypt the password
                if (!String.IsNullOrWhiteSpace(settings.AdminPassword))
                {
                    try
                    {                        
                        options.AdminPassword = _encrypter.Decrypt(settings.AdminPassword);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the demo administrator password. {e.Message}");
                        }
                    }
                }

            }

        }

    }

}
