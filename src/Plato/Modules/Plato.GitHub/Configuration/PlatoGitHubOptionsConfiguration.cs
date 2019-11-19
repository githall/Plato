using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.GitHub.Models;
using Plato.GitHub.Stores;

namespace Plato.GitHub.Configuration
{
    public class PlatoGitHubOptionsConfiguration : IConfigureOptions<PlatoGitHubOptions>
    {

        private readonly IGitHubSettingsStore<PlatoGitHubSettings> _githubSettingsStore;        
        private readonly ILogger<PlatoGitHubOptionsConfiguration> _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public PlatoGitHubOptionsConfiguration(
            IGitHubSettingsStore<PlatoGitHubSettings> githubSettingsStore,            
            ILogger<PlatoGitHubOptionsConfiguration> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _githubSettingsStore = githubSettingsStore;            
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoGitHubOptionsConfiguration));
                        options.ClientSecret = protector.Unprotect(settings.ClientSecret);
                    }
                    catch
                    {
                        _logger.LogError("There was a problem decrypting the SMTP password.");
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
