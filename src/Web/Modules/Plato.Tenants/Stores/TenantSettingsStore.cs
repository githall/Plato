using Microsoft.Extensions.Logging;
using Plato.Tenants.Models;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using System.Threading.Tasks;

namespace Plato.Tenants.Stores
{

    public class TenantSettingsStore : ITenantSettingsStore<DefaultTenantSettings>
    {
        private const string SettingsKey = "TenantSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<TenantSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public TenantSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<TenantSettingsStore> logger, 
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<DefaultTenantSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<DefaultTenantSettings>(SettingsKey));
        }

        public async Task<DefaultTenantSettings> SaveAsync(DefaultTenantSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<DefaultTenantSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Email settings updated");
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
                    _logger.LogInformation("Email settings deleted");
                }
            }

            return result;

        }

    }

}
