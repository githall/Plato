using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Twitter.Models;
using Plato.Twitter.Stores;
using Plato.Twitter.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Twitter.Configuration;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;

namespace Plato.Twitter.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<TwitterSettings>
    {

        private readonly ITwitterSettingsStore<TwitterSettings> _twitterSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            ITwitterSettingsStore<TwitterSettings> twitterSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _twitterSettingsStore = twitterSettingsStore;
            _platoOptions = platoOptionsAccessor.Value;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(TwitterSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(TwitterSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(TwitterSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<TwitterSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<TwitterSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<TwitterSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(TwitterSettings settings, IViewProviderContext context)
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        consumerSecret = protector.Protect(model.ConsumerSecret);
               
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.AccessTokenSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        accessTokenSecret = protector.Protect(model.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                // Create the model
                settings = new TwitterSettings()
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        consumerSecret = protector.Unprotect(settings.ConsumerSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                if (!string.IsNullOrWhiteSpace(settings.AccessTokenSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(TwitterOptionsConfiguration));
                        accessTokenSecret = protector.Unprotect(settings.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
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
