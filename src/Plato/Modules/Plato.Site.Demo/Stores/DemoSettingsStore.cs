using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Site.Demo.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Site.Demo.Stores
{
    
    public class DemoSettingsStore : IDemoSettingsStore<DemoSettings>
    {

        private const string SettingsKey = "DemoSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<DemoSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public DemoSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<DemoSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<DemoSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<DemoSettings>(SettingsKey));
        }

        public async Task<DemoSettings> SaveAsync(DemoSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Twitter settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<DemoSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Demo settings updated");
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
                    _logger.LogInformation("Demo settings deleted");
                }
            }

            return result;

        }
    }
}
