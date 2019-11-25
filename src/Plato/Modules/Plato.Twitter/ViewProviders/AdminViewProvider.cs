using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Twitter.Models;
using Plato.Twitter.Stores;
using Plato.Twitter.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Twitter.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<PlatoTwitterSettings>
    {

        private readonly ITwitterSettingsStore<PlatoTwitterSettings> _twitterSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IEncrypter _encrypter;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            ITwitterSettingsStore<PlatoTwitterSettings> twitterSettingsStore,
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {
            _twitterSettingsStore = twitterSettingsStore;
            _platoOptions = platoOptionsAccessor.Value;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _encrypter = encrypter;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(PlatoTwitterSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoTwitterSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoTwitterSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<TwitterSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<TwitterSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<TwitterSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoTwitterSettings settings, IViewProviderContext context)
        {

            var model = new TwitterSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var consumerSecret = string.Empty;
                var accessTokenSecret = string.Empty;

                if (!string.IsNullOrWhiteSpace(model.ConsumerSecret))
                {
                    try
                    {                        
                        consumerSecret = _encrypter.Encrypt(model.ConsumerSecret);               
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the Twitter consumer secret. {e.Message}");
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.AccessTokenSecret))
                {
                    try
                    {                        
                        accessTokenSecret = _encrypter.Encrypt(model.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the Twitter access token secret. {e.Message}");
                        }
                    }
                }

                // Create the model
                settings = new PlatoTwitterSettings()
                {
                    ConsumerKey = model.ConsumerKey,
                    ConsumerSecret = consumerSecret,
                    CallbackPath = model.CallbackPath,
                    AccessToken = model.AccessToken,
                    AccessTokenSecret = accessTokenSecret
                };

                // Persist the settings
                var result = await _twitterSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        async Task<TwitterSettingsViewModel> GetModel()
        {

            var settings = await _twitterSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var consumerSecret = string.Empty;
                var accessTokenSecret = string.Empty;

                if (!string.IsNullOrWhiteSpace(settings.ConsumerSecret))
                {
                    try
                    {
                        consumerSecret = _encrypter.Decrypt(settings.ConsumerSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Twitter consumer secret. {e.Message}");
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(settings.AccessTokenSecret))
                {
                    try
                    {                        
                        accessTokenSecret = _encrypter.Decrypt(settings.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError($"There was a problem decrypting the Twitter access token secret. {e.Message}");
                        }
                    }
                }

                return new TwitterSettingsViewModel()
                {
                    ConsumerKey = _platoOptions.DemoMode ? "123456789" : settings.ConsumerKey,
                    ConsumerSecret = _platoOptions.DemoMode ? "xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx" : consumerSecret,
                    CallbackPath = _platoOptions.DemoMode ? string.Empty : settings.CallbackPath.ToString(),
                    AccessToken = _platoOptions.DemoMode ? "123456789" : settings.AccessToken,
                    AccessTokenSecret = _platoOptions.DemoMode ? "xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx" : accessTokenSecret
                };

            }

            // return default settings
            return new TwitterSettingsViewModel();

        }

    }

}
