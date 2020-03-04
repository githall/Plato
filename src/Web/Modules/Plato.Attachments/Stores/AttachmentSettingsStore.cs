using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Attachments.Models;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;

namespace Plato.Attachments.Stores
{

    public class AttachmentSettingsStore : IAttachmentSettingsStore<AttachmentSettings>
    {

        private const string SettingsKey = "AttachmentSettings";

        private readonly ILogger<AttachmentSettingsStore> _logger;
        private readonly IDictionaryStore _dictionaryStore;        
        private readonly ICacheManager _cacheManager;

        public AttachmentSettingsStore(
            ILogger<AttachmentSettingsStore> logger,
            IDictionaryStore dictionaryStore,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<AttachmentSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<AttachmentSettings>(SettingsKey));
        }

        public async Task<AttachmentSettings> SaveAsync(AttachmentSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<AttachmentSettings>(SettingsKey, model);
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
