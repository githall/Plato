using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.GitHub.Models;
using Plato.GitHub.Stores;
using Plato.GitHub.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Security.Abstractions.Encryption;
using PlatoCore.Hosting.Abstractions;

namespace Plato.GitHub.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<PlatoGitHubSettings>
    {

        private readonly IGitHubSettingsStore<PlatoGitHubSettings> _gitHubSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IEncrypter _encrypter;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            IGitHubSettingsStore<PlatoGitHubSettings> gitHubSettingsStore,            
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IEncrypter encrypter,
            IPlatoHost platoHost)
        {
            _gitHubSettingsStore = gitHubSettingsStore;
            _platoOptions = platoOptionsAccessor.Value;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _encrypter = encrypter;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(PlatoGitHubSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoGitHubSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoGitHubSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<GoogleSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("header-right").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoGitHubSettings settings, IViewProviderContext context)
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
                        secret = _encrypter.Encrypt(model.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem encrypting the GitHub client secret. {e.Message}");
                        }
                    }
                }

                // Create the model
                settings = new PlatoGitHubSettings()
                {
                    ClientId = model.ClientId,
                    ClientSecret = secret,
                    CallbackPath = model.CallbackPath
                };

                // Persist the settings
                var result = await _gitHubSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        async Task<GoogleSettingsViewModel> GetModel()
        {

            var settings = await _gitHubSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {
                        secret = _encrypter.Decrypt(settings.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the GitHub client secret. {e.Message}");
                        }
                    }
                }

                return new GoogleSettingsViewModel()
                {
                    ClientId = _platoOptions.DemoMode ? "123456789" : settings.ClientId,
                    ClientSecret = _platoOptions.DemoMode ? "xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx" : secret,
                    CallbackPath = _platoOptions.DemoMode ? string.Empty : settings.CallbackPath.ToString()
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
