using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Security.Abstractions.Encryption;
using Plato.Slack.Models;
using Plato.Slack.Stores;
using Plato.Slack.ViewModels;

namespace Plato.Slack.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<PlatoSlackSettings>
    {

        private readonly ISlackSettingsStore<PlatoSlackSettings> _TwitterSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IEncrypter _encrypter;

        public AdminViewProvider(
            ISlackSettingsStore<PlatoSlackSettings> TwitterSettingsStore,            
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {            
            _TwitterSettingsStore = TwitterSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _encrypter = encrypter;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(PlatoSlackSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoSlackSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoSlackSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<SlackSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<SlackSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<SlackSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoSlackSettings settings, IViewProviderContext context)
        {

            var model = new SlackSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var webHookUrl = string.Empty;
                var accessTokenSecret = string.Empty;

                if (!string.IsNullOrWhiteSpace(model.WebHookUrl))
                {
                    try
                    {
                        webHookUrl = _encrypter.Encrypt(model.WebHookUrl);               
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the Slack Web Hook URL. {e.Message}");
                        }
                    }
                }

                // Create the model
                settings = new PlatoSlackSettings()
                {
                    WebHookUrl = webHookUrl
                };

                // Persist the settings
                var result = await _TwitterSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        async Task<SlackSettingsViewModel> GetModel()
        {

            var settings = await _TwitterSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var webHookUrl = string.Empty;

                if (!string.IsNullOrWhiteSpace(settings.WebHookUrl))
                {
                    try
                    {
                        webHookUrl = _encrypter.Decrypt(settings.WebHookUrl);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError($"There was a problem dencrypting the Slack Web Hook URL. {e.Message}");
                        }
                    }
                }

                return new SlackSettingsViewModel()
                {
                    WebHookUrl = webHookUrl
                };

            }

            // return default settings
            return new SlackSettingsViewModel();

        }

    }

}
