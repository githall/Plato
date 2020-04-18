﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Stores.Abstractions.Settings;

namespace Plato.Core.Configuration
{

    public class SiteOptionsConfiguration : IConfigureOptions<SiteOptions>
    {

        //private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SiteOptionsConfiguration(
            IServiceScopeFactory serviceScopeFactory,
            ISiteSettingsStore siteSettingsStore)
        {         
            _serviceScopeFactory = serviceScopeFactory;
            //_siteSettingsStore = siteSettingsStore;
        }

        public void Configure(SiteOptions options)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var shellSettingsStore = scope.ServiceProvider.GetRequiredService<ISiteSettingsStore>();

                var settings = shellSettingsStore
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();

                if (settings != null)
                {
                    options.SiteName = settings.SiteName;
                    options.Culture = settings.Culture;
                    options.DateTimeFormat = settings.DateTimeFormat;
                    options.Theme = settings.Theme;
                    options.TimeZone = settings.TimeZone;
                    options.HomeRoute = settings.HomeRoute ?? new HomeRoute();
                    options.HomeAlias = settings.HomeAlias;
                }
            
            }

        }

    }

}
