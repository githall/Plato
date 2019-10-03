using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.Stores;
using Plato.Site.Demo.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Site.Demo.Configuration;

namespace Plato.Site.Demo.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<DemoSettings>
    {

        private readonly IDemoSettingsStore<DemoSettings> _demoSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            IDemoSettingsStore<DemoSettings> demoSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _demoSettingsStore = demoSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
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
                View<DemoSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(DemoOptionsConfiguration));
                        adminPassword = protector.Protect(model.AdminPassword);

                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
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
                    _platoHost.RecycleShellContext(_shellSettings);
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(DemoOptionsConfiguration));
                        adminPassword = protector.Unprotect(settings.AdminPassword);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                return new DemoSettingsViewModel()
                {
                    AdminUserName = settings.AdminPassword,
                    AdminPassword = adminPassword
                };

            }

            // return default settings
            return new DemoSettingsViewModel();

        }
        
    }

}
