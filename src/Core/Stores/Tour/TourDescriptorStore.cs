using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using PlatoCore.Stores.Abstractions.Tour;
using PlatoCore.Models.Tour;

namespace PlatoCore.Stores.Tour
{

    public class TourDescriptorStore : ITourDescriptorStore
    {

        private const string Key = "TourDescriptor";

        private readonly ILogger<TourDescriptorStore> _logger;
        private readonly IDictionaryStore _dictionaryStore;
        private readonly ICacheManager _cacheManager;

        public TourDescriptorStore(
            ILogger<TourDescriptorStore> logger,
            IDictionaryStore dictionaryStore,            
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;            
            _cacheManager = cacheManager;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TourDescriptor> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), Key);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => 
                await _dictionaryStore.GetAsync<TourDescriptor>(Key));
        }

        public async Task<TourDescriptor> SaveAsync(TourDescriptor shellDescriptor)
        {
    
            // Update descriptor
            var descriptor = await _dictionaryStore.UpdateAsync<TourDescriptor>(Key, shellDescriptor);
            if (descriptor != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Shell descriptor updated successfully");
                }
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }

            return descriptor;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(Key);
            if (result)
            {
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }
            return result;
        }

        #endregion
        
    }

}
