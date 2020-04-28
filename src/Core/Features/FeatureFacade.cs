﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Stores.Abstractions.Shell;

namespace PlatoCore.Features
{

    public class FeatureFacade : IFeatureFacade
    {
        
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly IShellDescriptorManager _shellDescriptorManager;

        public FeatureFacade(
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IShellDescriptorManager shellDescriptorManager)
        {
            _shellFeatureStore = shellFeatureStore;
            _shellDescriptorManager = shellDescriptorManager;
        }

        public async Task<IShellFeature> GetFeatureByIdAsync(string moduleId)
        {
            var features = await _shellFeatureStore.SelectFeatures();
            return features?.FirstOrDefault(f => f.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns available feature update. 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IShellFeature>> GetFeatureUpdatesAsync()
        {

            // Get all features from the file system
            var modules = await _shellDescriptorManager
                .GetFeaturesAsync();
            var modulesList = modules.ToList();

            // Get enabled features from database
            var features = await _shellFeatureStore
                .QueryAsync()
                .ToList();
            
            // Iterate all enabled features from shell features tables
            // checking if a newer version exists on the file system
            List<IShellFeature> output = null;
            foreach (var feature in features.Data)
            {

                // Get available module descriptor
                var module = modulesList.FirstOrDefault(m => m.ModuleId.Equals(feature.ModuleId));

                // Ensure the module is available
                if (module != null)
                {
     
                    // Available module version
                    var moduleVersion = module.Descriptor.Version.ToVersion();

                    // Enabled feature version
                    var featureVersion = feature.Version.ToVersion();

                    // Ensure we have versions to compare
                    if (moduleVersion != null && featureVersion != null)
                    {
                        // The module is newer than the installed feature
                        if (moduleVersion > featureVersion)
                        {
                            if (output == null)
                            {
                                output = new List<IShellFeature>();;
                            }
                            output.Add(module);
                        }
                    }

                }
                
            }

            return output;

        }

    }

}
