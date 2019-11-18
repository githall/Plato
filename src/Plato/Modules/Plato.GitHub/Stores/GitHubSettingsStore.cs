using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.GitHub.Models;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Stores.Abstract;

namespace Plato.GitHub.Stores
{
    
    public class GitHubSettingsStore : IGitHubSettingsStore<GitHubSettings>
    {

        private const string SettingsKey = "GitHubSettings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<GitHubSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public GitHubSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<GitHubSettingsStore> logger,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }
        public async Task<GitHubSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<GitHubSettings>(SettingsKey));
        }

        public async Task<GitHubSettings> SaveAsync(GitHubSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Google settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<GitHubSettings>(SettingsKey, model);
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
