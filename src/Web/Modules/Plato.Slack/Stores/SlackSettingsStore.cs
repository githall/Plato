using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using Plato.Slack.Models;

namespace Plato.Slack.Stores
{
    
    public class SlackSettingsStore : ISlackSettingsStore<PlatoSlackSettings>
    {

        private const string SettingsKey = "SlackSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<SlackSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public SlackSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<SlackSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<PlatoSlackSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<PlatoSlackSettings>(SettingsKey));
        }

        public async Task<PlatoSlackSettings> SaveAsync(PlatoSlackSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Slack settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<PlatoSlackSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Slack settings updated");
                }

            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(SettingsKey);
            if (result)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Slack settings deleted");
                }
            }

            return result;

        }
    }
}
