﻿using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Models.Shell;
using PlatoCore.Stores.Abstract;
using PlatoCore.Stores.Abstractions.Shell;

namespace PlatoCore.Stores.Shell
{

    public class ShellDescriptorStore : IShellDescriptorStore
    {

        private const string Key = "ShellDescriptor";

        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly ILogger<ShellDescriptorStore> _logger;
        private readonly IDictionaryStore _dictionaryStore;
        private readonly ICacheManager _cacheManager;

        public ShellDescriptorStore(
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            ILogger<ShellDescriptorStore> logger,
            IDictionaryStore dictionaryStore,            
            ICacheManager cacheManager)
        {
            _shellFeatureStore = shellFeatureStore;
            _dictionaryStore = dictionaryStore;            
            _cacheManager = cacheManager;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<IShellDescriptor> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), Key);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<ShellDescriptor>(Key));
        }

        public async Task<IShellDescriptor> SaveAsync(IShellDescriptor shellDescriptor)
        {
            
            // Add & get features
            var features = await AddAndGetFeaturesAsync(shellDescriptor.Modules);;
            shellDescriptor.Modules = features.ToList();
            
            // Update descriptor
            var descriptor = await _dictionaryStore.UpdateAsync<ShellDescriptor>(Key, shellDescriptor);
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

        #region "Private Methods"

        async Task<IEnumerable<ShellModule>> AddAndGetFeaturesAsync(IEnumerable<ShellModule> modulesToAdd)
        {

            // Ensure we have a distinct list of features to save
            var distinctDictionary = new ConcurrentDictionary<string, ShellModule>();
            foreach (var module in modulesToAdd)
            {
                if (!distinctDictionary.ContainsKey(module.ModuleId))
                {
                    distinctDictionary.TryAdd(module.ModuleId, module);
                }
            }

            // Distinct list of features to add
            var featuresToAdd = distinctDictionary.Values.ToList();

            // Get all currently registered features
            var currentFeatures = await _shellFeatureStore.SelectFeatures();
            
            // Convert to list
            var currentFeaturesList = currentFeatures.ToList();

            // Delete any feature not in our current list of features to add
            foreach (var feature in currentFeaturesList)
            {
                if (featuresToAdd.FirstOrDefault(f => f.ModuleId == feature.ModuleId) == null)
                {
                    await _shellFeatureStore.DeleteAsync(feature);
                }
            }
            
            // Add any new features into our features table
            var output = new List<ShellModule>();
            foreach (var feature in featuresToAdd)
            {
                // add or update feature
                var existingFeature = currentFeaturesList.FirstOrDefault(f => f.ModuleId == feature.ModuleId)
                                      ?? new ShellFeature()
                                      {
                                          ModuleId = feature.ModuleId,
                                          Version = feature.Version,
                                          FeatureSettings = new FeatureSettings()
                                      };

                var newOrUpdatedFeature = await _shellFeatureStore.CreateAsync(existingFeature);
                if (newOrUpdatedFeature != null)
                {
                    output.Add(new ShellModule(newOrUpdatedFeature));
                }

            }
            
            return output;

        }

        #endregion
        
    }

}
