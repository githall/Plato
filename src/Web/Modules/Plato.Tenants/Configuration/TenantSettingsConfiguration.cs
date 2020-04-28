using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Tenants.Models;
using Plato.Tenants.Stores;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Tenants.Configuration
{

    public class TenantSettingsConfiguration : IConfigureOptions<DefaultTenantSettings>
    {

        private readonly ITenantSettingsStore<DefaultTenantSettings> _emailSettingsStore;
        private readonly ILogger<TenantSettingsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public TenantSettingsConfiguration(
            ITenantSettingsStore<DefaultTenantSettings> emailSettingsStore,            
            ILogger<TenantSettingsConfiguration> logger,
            IEncrypter encrypter)
        {
            _emailSettingsStore = emailSettingsStore;
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(DefaultTenantSettings options)
        {

            var settings = _emailSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            // We have no settings to configure

            if (settings != null)
            {

                // Decrypt the connection string 
                if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    try
                    {
                        options.ConnectionString = _encrypter.Decrypt(settings.ConnectionString);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the default tenant connection string. {e.Message}");
                        }
                    }
                }

                options.TablePrefix = settings.TablePrefix;

                var smtpSettings = settings.SmtpSettings;
                if (smtpSettings != null)
                {

                    options.SmtpSettings.DefaultFrom = smtpSettings.DefaultFrom;
                    options.SmtpSettings.DeliveryMethod = smtpSettings.DeliveryMethod;
                    options.SmtpSettings.PickupDirectoryLocation = smtpSettings.PickupDirectoryLocation;
                    options.SmtpSettings.Host = smtpSettings.Host;
                    options.SmtpSettings.Port = smtpSettings.Port;
                    options.SmtpSettings.EnableSsl = smtpSettings.EnableSsl;
                    options.SmtpSettings.RequireCredentials = smtpSettings.RequireCredentials;
                    options.SmtpSettings.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
                    options.SmtpSettings.UserName = smtpSettings.UserName;

                    // Decrypt the password
                    if (!String.IsNullOrWhiteSpace(smtpSettings.Password))
                    {
                        try
                        {
                            options.SmtpSettings.Password = _encrypter.Decrypt(smtpSettings.Password);
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

}
