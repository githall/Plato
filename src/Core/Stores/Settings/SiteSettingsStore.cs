﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Cache;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Text.Abstractions;

namespace PlatoCore.Stores.Settings
{
    public class SiteSettingsStore : ISiteSettingsStore
    {

        private const string Key = "SiteSettings";

        private readonly ILogger<SiteSettingsStore> _logger;
        private readonly IDictionaryStore _dictionaryStore;

        private readonly IKeyGenerator _keyGenerator;
        private readonly ICacheManager _cacheManager;

        public SiteSettingsStore(
            ILogger<SiteSettingsStore> logger,
            IDictionaryStore dictionaryStore,     
            ICacheManager cacheManager,             
            IKeyGenerator keyGenerator)
        {
            _dictionaryStore = dictionaryStore;        
            _keyGenerator = keyGenerator;
            _cacheManager = cacheManager;            
            _logger = logger;    
        }

        public async Task<ISiteSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), Key);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<SiteSettings>(Key));
        }

        public async Task<ISiteSettings> SaveAsync(ISiteSettings siteSettings)
        {

            // Automatically generate an API key if one is not supplied
            if (String.IsNullOrWhiteSpace(siteSettings.ApiKey))
            {
                siteSettings.ApiKey = _keyGenerator.GenerateKey();
            }

            // Use default homepage route if a default route is not explicitly specified
            if (siteSettings.HomeRoute == null)
            {
                siteSettings.HomeRoute = new HomeRoute();
            }
            
            // Update settings
            var settings = await _dictionaryStore.UpdateAsync<SiteSettings>(Key, siteSettings);
            if (settings != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Settings for site '{settings.SiteName}' updated successfully");
                }
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }

            return settings;
        }

        public async Task<bool> DeleteAsync()
        {
            var result =  await _dictionaryStore.DeleteAsync(Key);
            if (result)
            {
                // Expire cache
                _cacheManager.CancelTokens(this.GetType(), Key);
            }
            
            return result;
        }
        
    }
}
