using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.GitHub.Models;
using Plato.GitHub.Stores;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.GitHub.Configuration
{
    public class PlatoGitHubOptionsConfiguration : IConfigureOptions<PlatoGitHubOptions>
    {

        private readonly IGitHubSettingsStore<PlatoGitHubSettings> _githubSettingsStore;        
        private readonly ILogger<PlatoGitHubOptionsConfiguration> _logger;        
        private readonly IEncrypter _encrypter;

        public PlatoGitHubOptionsConfiguration(
            IGitHubSettingsStore<PlatoGitHubSettings> githubSettingsStore,            
            ILogger<PlatoGitHubOptionsConfiguration> logger,            
            IEncrypter encrypter)
        {            
            _githubSettingsStore = githubSettingsStore;
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(PlatoGitHubOptions options)
        {

            var settings = _githubSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.ClientId = settings.ClientId;

                // Decrypt the secret
                if (!String.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {                        
                        options.ClientSecret = _encrypter.Decrypt(settings.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the GitHub client secret. {e.Message}");
                        }
                    }
                }

                if (settings.CallbackPath.HasValue)
                {
                    options.CallbackPath = settings.CallbackPath;
                }

            }

        }

    }

}
