﻿using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Stores.Abstract;
using Plato.Users.reCAPTCHA2.Models;

namespace Plato.Users.reCAPTCHA2.Stores
{
    
    public class ReCaptchaSettingsStore : IReCaptchaSettingsStore<ReCaptchaSettings>
    {

        private const string SettingsKey = "ReCaptcha2Settings";

        private readonly IDictionaryStore _dictionaryStore;
        private readonly ILogger<ReCaptchaSettingsStore> _logger;
        private readonly ICacheManager _cacheManager;

        public ReCaptchaSettingsStore(
            IDictionaryStore dictionaryStore,
            ILogger<ReCaptchaSettingsStore> logger,
            IMemoryCache memoryCache,
            ICacheManager cacheManager)
        {
            _dictionaryStore = dictionaryStore;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<ReCaptchaSettings> GetAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _dictionaryStore.GetAsync<ReCaptchaSettings>(SettingsKey));
        }

        public async Task<ReCaptchaSettings> SaveAsync(ReCaptchaSettings model)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("reCAPTCHA2 settings updating");
            }

            var settings = await _dictionaryStore.UpdateAsync<ReCaptchaSettings>(SettingsKey, model);
            if (settings != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("reCAPTCHA2 settings updated");
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
                    _logger.LogInformation("reCAPTCHA2 settings deleted");
                }
            }

            return result;

        }

    }

}
