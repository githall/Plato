using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using PlatoCore.Stores.Abstractions.Tour;
using PlatoCore.Models.Tour;
using System;
using System.Collections.Generic;

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
            {
                var descriptor = await _dictionaryStore.GetAsync<TourDescriptor>(Key);
                if (descriptor != null)
                {
                    return descriptor;
                }
                return new DefaultTourDescriptor();
            });
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

        public async Task<TourStep> GetStepAsync(string id)
        {

            if (string.IsNullOrEmpty(id)) 
            {
                throw new ArgumentNullException(nameof(id));

            }
            
            // Get descriptor
            var descriptor = await GetAsync();

            // Return step
            if (descriptor.Steps != null)
            {
                foreach (var step in descriptor.Steps)
                {
                    if (id.Equals(step.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        return step;
                    }
                }
            }

            return null;

        }

        public async Task<TourDescriptor> UpdateStepAsync(TourStep model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Get descriptor
            var descriptor = await GetAsync();

            var steps = new List<TourStep>();
            if (descriptor.Steps != null)
            {                
                foreach (var step in descriptor.Steps)
                {
                    if (model.Id.Equals(step.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        steps.Add(model);
                    }
                    else
                    {
                        steps.Add(step);
                    }
                }
            }

            descriptor.Steps = steps;

            return await SaveAsync(descriptor);

        }

        #endregion

    }

}
