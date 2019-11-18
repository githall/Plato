using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.GitHub.Models;
using Plato.GitHub.Stores;

namespace Plato.GitHub.Configuration
{
    public class GitHubAuthenticationOptionsConfiguration : IConfigureOptions<GitHubAuthenticationOptions>
    {

        private readonly IGitHubSettingsStore<GitHubSettings> _googleSettingsStore;        
        private readonly ILogger<GitHubAuthenticationOptionsConfiguration> _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public GitHubAuthenticationOptionsConfiguration(
            IGitHubSettingsStore<GitHubSettings> googleSettingsStore,            
            ILogger<GitHubAuthenticationOptionsConfiguration> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider; 
            _googleSettingsStore = googleSettingsStore;            
            _logger = logger;
        }

        public void Configure(GitHubAuthenticationOptions options)
        {

            var settings = _googleSettingsStore
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
                        var protector = _dataProtectionProvider.CreateProtector(nameof(GitHubAuthenticationOptionsConfiguration));
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
