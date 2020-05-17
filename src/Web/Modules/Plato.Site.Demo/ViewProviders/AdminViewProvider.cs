using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.Stores;
using Plato.Site.Demo.ViewModels;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Site.Demo.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<DemoSettings>
    {

        private readonly IDemoSettingsStore<DemoSettings> _demoSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IEncrypter _encrypter;

        public AdminViewProvider(
            IDemoSettingsStore<DemoSettings> demoSettingsStore,            
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {
            _demoSettingsStore = demoSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _encrypter = encrypter;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(DemoSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(DemoSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(DemoSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<DemoSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<DemoSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("header-right").Order(1),
                View<DemoSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(DemoSettings settings, IViewProviderContext context)
        {
            
            var model = new DemoSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the password
                var adminPassword = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.AdminPassword))
                {
                    try
                    {
                        adminPassword = _encrypter.Encrypt(model.AdminPassword);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the demo administrator password. {e.Message}");
                        }
                    }
                }

                // Create the model
                settings = new DemoSettings()
                {
                    AdminUserName = model.AdminUserName,
                    AdminPassword = adminPassword
                };

                // Persist the settings
                var result = await _demoSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        async Task<DemoSettingsViewModel> GetModel()
        {

            var settings = await _demoSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the password
                var adminPassword = string.Empty;

                if (!string.IsNullOrWhiteSpace(settings.AdminPassword))
                {
                    try
                    {                        
                        adminPassword = _encrypter.Decrypt(settings.AdminPassword);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the demo administrator password. {e.Message}");
                        }
                    }
                }

                return new DemoSettingsViewModel()
                {
                    AdminUserName = settings.AdminUserName,
                    AdminPassword = adminPassword
                };

            }

            // return default settings
            return new DemoSettingsViewModel();

        }
        
    }

}
