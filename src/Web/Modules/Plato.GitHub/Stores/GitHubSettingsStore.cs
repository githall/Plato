using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.GitHub.Models;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;

namespace Plato.GitHub.Stores
{
    
    public class GitHubSettingsStore : IGitHubSettingsStore<PlatoGitHubSettings>
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
        public async Task<PlatoGitHubSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<PlatoGitHubSettings>(SettingsKey));
        }

        public async Task<PlatoGitHubSettings> SaveAsync(PlatoGitHubSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Google settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<PlatoGitHubSettings>(SettingsKey, model);
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
