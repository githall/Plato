using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Security.Abstractions.Encryption;
using Plato.Slack.Models;
using Plato.Slack.Stores;

namespace Plato.Slack.Configuration
{
    public class SlackOptionsConfiguration : IConfigureOptions<PlatoSlackOptions>
    {

        private readonly ISlackSettingsStore<PlatoSlackSettings> _slackSettingsStore;        
        private readonly ILogger<SlackOptionsConfiguration> _logger;
        private readonly IEncrypter _encrypter;

        public SlackOptionsConfiguration(
            ISlackSettingsStore<PlatoSlackSettings> slackSettingsStore,
            ILogger<SlackOptionsConfiguration> logger,
            IEncrypter encrypter)
        {
            _slackSettingsStore = slackSettingsStore;            
            _encrypter = encrypter;
            _logger = logger;
        }

        public void Configure(PlatoSlackOptions options)
        {

            var settings = _slackSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {                
                if (!String.IsNullOrWhiteSpace(settings.WebHookUrl))
                {
                    try
                    {                        
                        options.WebHookUrl = _encrypter.Decrypt(settings.WebHookUrl);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e, $"There was a problem decrypting the Slack Web Hook URL. {e.Message}");
                        }                        
                    }
                }
            }

        }

    }

}
