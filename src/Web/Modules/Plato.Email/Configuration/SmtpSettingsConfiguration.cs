using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Stores;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Email.Configuration
{

    public class SmtpSettingsConfiguration : IConfigureOptions<SmtpSettings>
    {

        private readonly IEmailSettingsStore<EmailSettings> _emailSettingsStore;
        private readonly ILogger<SmtpSettingsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public SmtpSettingsConfiguration(
            IEmailSettingsStore<EmailSettings> emailSettingsStore,            
            ILogger<SmtpSettingsConfiguration> logger,
            IEncrypter encrypter)
        {
            _emailSettingsStore = emailSettingsStore;
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(SmtpSettings options)
        {

            var settings = _emailSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            // We have no settings to configure

            var smtpSettings = settings?.SmtpSettings;
            if (smtpSettings != null)
            {

                options.DefaultFrom = smtpSettings.DefaultFrom;
                options.DeliveryMethod = smtpSettings.DeliveryMethod;
                options.PickupDirectoryLocation = smtpSettings.PickupDirectoryLocation;
                options.Host = smtpSettings.Host;
                options.Port = smtpSettings.Port;
                options.EnableSsl = smtpSettings.EnableSsl;
                options.RequireCredentials = smtpSettings.RequireCredentials;
                options.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
                options.UserName = smtpSettings.UserName;

                // Decrypt the password
                if (!String.IsNullOrWhiteSpace(smtpSettings.Password))
                {
                    try
                    {                        
                        options.Password = _encrypter.Decrypt(smtpSettings.Password);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the SMTP password. {e.Message}");
                        }
                    }
                }

            }

        }

    }

}
