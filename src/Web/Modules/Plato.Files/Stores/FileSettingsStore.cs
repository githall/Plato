using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Files.Models;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;

namespace Plato.Files.Stores
{

    public class FileSettingsStore : IFileSettingsStore<FileSettings>
    {

        private const string SettingsKey = "FileSettings";

        private readonly ILogger<FileSettingsStore> _logger;
        private readonly IDictionaryStore _dictionaryStore;        
        private readonly ICacheManager _cacheManager;

        public FileSettingsStore(
            ILogger<FileSettingsStore> logger,
            IDictionaryStore dictionaryStore,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<FileSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<FileSettings>(SettingsKey));
        }

        public async Task<FileSettings> SaveAsync(FileSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<FileSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Attachment settings updated");
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
                    _logger.LogInformation("Attachment settings deleted");
                }
            }

            return result;

        }

    }

}
