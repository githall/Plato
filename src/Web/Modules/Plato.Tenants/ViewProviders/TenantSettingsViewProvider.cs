using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Tenants.Models;
using Plato.Tenants.Stores;
using PlatoCore.Models.Shell;
using Plato.Tenants.ViewModels;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Security.Abstractions.Encryption;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Tenants.ViewProviders
{

    public class TenantSettingsViewProvider : ViewProviderBase<DefaultTenantSettings>
    {

        private readonly ITenantSettingsStore<DefaultTenantSettings> _tenantSettingsStore;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IEncrypter _encrypter;
        private readonly IPlatoHost _platoHost;

        private readonly PlatoOptions _platoOptions;

        public TenantSettingsViewProvider(
            ITenantSettingsStore<DefaultTenantSettings> tenantSettingsStore,  
            IOptions<PlatoOptions> platoOptions,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {
            _tenantSettingsStore = tenantSettingsStore;
            _platoOptions = platoOptions.Value;
            _shellSettings = shellSettings;
            _encrypter = encrypter;
            _platoHost = platoHost;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(DefaultTenantSettings settings, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(DefaultTenantSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(DefaultTenantSettings settings, IViewProviderContext updater)
        {

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Build view model
            var viewModel = await GetModel();
            return Views(
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Header", model => viewModel).Zone("header"),
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Content", model => viewModel).Zone("content"),
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Footer", model => viewModel).Zone("footer")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(DefaultTenantSettings settings, IViewProviderContext context)
        {

            var model = new EditTenantSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt connection string    
                var connectionString = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.ConnectionString))
                {
                    try
                    {
                        connectionString = _encrypter.Encrypt(model.ConnectionString);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError($"There was a problem encrypting the default tenant connection string. {e.Message}");
                        }
                    }
                }

                // Encrypt password    
                var password = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.SmtpSettings.Password))
                {
                    try
                    {
                        password = _encrypter.Encrypt(model.SmtpSettings.Password);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError($"There was a problem encrypting the SMTP server password. {e.Message}");
                        }
                    }
                }

                settings = new DefaultTenantSettings()
                {
                    ConnectionString = connectionString,
                    TablePrefix = model.TablePrefix,
                    SmtpSettings = new SmtpSettings()
                    {
                        DefaultFrom = model.SmtpSettings.DefaultFrom,
                        Host = model.SmtpSettings.Host,
                        Port = model.SmtpSettings.Port,
                        UserName = model.SmtpSettings.UserName,
                        Password = password,
                        RequireCredentials = model.SmtpSettings.RequireCredentials,
                        EnableSsl = model.SmtpSettings.EnableSsl
                    }
                };

                var result = await _tenantSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        // -----------------

        private async Task<EditTenantSettingsViewModel> GetModel()
        {

            var settings = await _tenantSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt connection string
                var connectionString = string.Empty;
                if (!String.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    try
                    {
                        connectionString = _encrypter.Decrypt(settings.ConnectionString);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the default tenant connection string. {e.Message}");
                        }
                    }
                }

                // Decrypt password
                var password = string.Empty;
                if (!String.IsNullOrWhiteSpace(settings.SmtpSettings.Password))
                {
                    try
                    {
                        password = _encrypter.Decrypt(settings.SmtpSettings.Password);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the default tenant connection string. {e.Message}");
                        }
                    }
                }

                return new EditTenantSettingsViewModel()
                {
                    ConnectionString = connectionString,
                    TablePrefix = settings.TablePrefix,
                    SmtpSettings = new SmtpSettingsViewModel()
                    {
                        DefaultFrom = _platoOptions.DemoMode
                            ? "email@example.com"
                            : settings.SmtpSettings.DefaultFrom,
                        Host = _platoOptions.DemoMode
                            ? "smtp.example.com"
                            : settings.SmtpSettings.Host,
                        Port = settings.SmtpSettings.Port,
                        UserName = _platoOptions.DemoMode
                            ? "email@example.com"
                            : settings.SmtpSettings.UserName,
                        Password = _platoOptions.DemoMode
                            ? ""
                            : password,
                        RequireCredentials = settings.SmtpSettings.RequireCredentials,
                        EnableSsl = settings.SmtpSettings.EnableSsl                        
                    }
                };
            }

            return new EditTenantSettingsViewModel();

        }

    }

}

