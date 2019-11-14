using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Google.Models;
using Plato.Google.Stores;
using Plato.Google.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Google.Configuration;

namespace Plato.Google.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<GoogleSettings>
    {

        private readonly IGoogleSettingsStore<GoogleSettings> _facebookSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            IGoogleSettingsStore<GoogleSettings> facebookSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _facebookSettingsStore = facebookSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(GoogleSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(GoogleSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(GoogleSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<GoogleSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(GoogleSettings settings, IViewProviderContext context)
        {
            
            var model = new GoogleSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(GoogleOptionsConfiguration));
                        secret = protector.Protect(model.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Google client secret. {e.Message}");
                    }
                }
                
                // Create the model
                settings = new GoogleSettings()
                {
                    AppId = model.ClientId,
                    AppSecret = secret
                };

                // Persist the settings
                var result = await _facebookSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }
              
            }

            return await BuildEditAsync(settings, context);

        }
        
        async Task<GoogleSettingsViewModel> GetModel()
        {

            var settings = await _facebookSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(settings.AppSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(GoogleOptionsConfiguration));
                        secret = protector.Unprotect(settings.AppSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Facebook app secret. {e.Message}");
                    }
                }


                return new GoogleSettingsViewModel()
                {
                    ClientId = settings.AppId,
                    ClientSecret = secret
                };
            }

            // return default settings
            return new GoogleSettingsViewModel()
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty
            };

        }
        
    }

}
