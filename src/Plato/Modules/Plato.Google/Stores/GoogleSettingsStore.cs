using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Google.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Google.Stores
{
    
    public class GoogleSettingsStore : IGoogleSettingsStore<GoogleSettings>
    {

        private const string SettingsKey = "GoogleSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<GoogleSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public GoogleSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<GoogleSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<GoogleSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<GoogleSettings>(SettingsKey));
        }

        public async Task<GoogleSettings> SaveAsync(GoogleSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Google settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<GoogleSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Facebook settings updated");
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
                    _logger.LogInformation("Google settings deleted");
                }
            }

            return result;

        }
    }
}
