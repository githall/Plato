using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Site.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.Site.Stores
{
    
    public class PlatoSiteSettingsStore : IPlatoSiteSettingsStore<PlatoSiteSettings>
    {

        private const string SettingsKey = "PlatoSiteSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<PlatoSiteSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public PlatoSiteSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<PlatoSiteSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<PlatoSiteSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<PlatoSiteSettings>(SettingsKey));
        }

        public async Task<PlatoSiteSettings> SaveAsync(PlatoSiteSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Plato site settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<PlatoSiteSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Plato site settings updated");
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
                    _logger.LogInformation("Plato site settings deleted");
                }
            }

            return result;

        }
    }
}
