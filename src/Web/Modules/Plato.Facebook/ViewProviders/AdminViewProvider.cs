using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;
using Plato.Facebook.ViewModels;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Abstractions.Settings;
using Microsoft.Extensions.Options;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Facebook.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<PlatoFacebookSettings>
    {

        private readonly IFacebookSettingsStore<PlatoFacebookSettings> _facebookSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IEncrypter _encrypter;
        private readonly IPlatoHost _platoHost;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            IFacebookSettingsStore<PlatoFacebookSettings> facebookSettingsStore,            
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {            
            _facebookSettingsStore = facebookSettingsStore;
            _platoOptions = platoOptionsAccessor.Value;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _encrypter = encrypter;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(PlatoFacebookSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoFacebookSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoFacebookSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<FacebookSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoFacebookSettings settings, IViewProviderContext context)
        {
            
            var model = new FacebookSettingsViewModel();

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
                if (!string.IsNullOrWhiteSpace(model.AppSecret))
                {
                    try
                    {
                        
                        secret = _encrypter.Encrypt(model.AppSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the Facebook app secret. {e.Message}");
                        }
                    }
                }

                // Create the model
                settings = new PlatoFacebookSettings()
                {
                    AppId = model.AppId,
                    AppSecret = secret,
                    CallbackPath = model.CallbackPath
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

        async Task<FacebookSettingsViewModel> GetModel()
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
                        secret = _encrypter.Decrypt(settings.AppSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Facebook app secret. {e.Message}");
                        }
                    }
                }

                return new FacebookSettingsViewModel()
                {
                    AppId = _platoOptions.DemoMode ? "123456789" : settings.AppId,
                    AppSecret = _platoOptions.DemoMode ? "xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx" : secret,
                    CallbackPath = _platoOptions.DemoMode ? string.Empty : settings.CallbackPath.ToString()
                };

            }

            // return default settings
            return new FacebookSettingsViewModel()
            {
                AppId = string.Empty,
                AppSecret = string.Empty
            };

        }

    }

}
