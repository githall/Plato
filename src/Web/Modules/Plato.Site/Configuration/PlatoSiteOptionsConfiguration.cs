using Microsoft.Extensions.Options;
using Plato.Site.Models;
using Plato.Site.Stores;

namespace Plato.Site.Configuration
{

    public class PlatoSiteOptionsConfiguration : IConfigureOptions<PlatoSiteOptions>
    {

        private readonly IPlatoSiteSettingsStore<PlatoSiteSettings> _platoSiteSettingsStore;                

        public PlatoSiteOptionsConfiguration(IPlatoSiteSettingsStore<PlatoSiteSettings> platoSiteSettingsStore)
        {
            _platoSiteSettingsStore = platoSiteSettingsStore;
        }

        public void Configure(PlatoSiteOptions options)
        {

            var settings = _platoSiteSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.HostUrl = settings.HostUrl;
                options.DemoUrl = settings.DemoUrl;
                options.PlatoDesktopUrl = settings.PlatoDesktopUrl;
            }


            
        }

    }

}
