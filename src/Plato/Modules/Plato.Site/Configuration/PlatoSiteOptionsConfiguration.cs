using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using Plato.Site.Models;
using Plato.Site.Stores;

namespace Plato.Site.Configuration
{

    public class PlatoSiteOptionsConfiguration : IConfigureOptions<PlatoSiteOptions>
    {

        private readonly IPlatoSiteSettingsStore<PlatoSiteSettings> _platoSiteSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<PlatoSiteOptionsConfiguration> _logger;

        public PlatoSiteOptionsConfiguration(
            IPlatoSiteSettingsStore<PlatoSiteSettings> platoSiteSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<PlatoSiteOptionsConfiguration> logger)
        {
            _platoSiteSettingsStore = platoSiteSettingsStore;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(PlatoSiteOptions options)
        {

            var settings = _platoSiteSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {

                options.DemoUrl = settings.DemoUrl;

            }

        }

    }

}
