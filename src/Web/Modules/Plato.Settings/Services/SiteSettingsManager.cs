using System.Threading.Tasks;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;

namespace Plato.Settings.Services
{

    public class SiteSettingsManager : ISiteSettingsManager
    {

        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IBroker _broker;

        public SiteSettingsManager(
            ISiteSettingsStore siteSettingsStore,
            IBroker broker)
        {
            _siteSettingsStore = siteSettingsStore;
            _broker = broker;
        }

        public async Task<ISiteSettings> SaveAsync(ISiteSettings settings)
        {

            // Invoke SiteSettingsUpdating subscriptions
            foreach (var handler in _broker.Pub<ISiteSettings>(this, "SiteSettingsUpdating"))
            {
                settings = await handler.Invoke(new Message<ISiteSettings>(settings, this));
            }

            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {

                // Invoke SiteSettingsUpdated subscriptions
                foreach (var handler in _broker.Pub<ISiteSettings>(this, "SiteSettingsUpdated"))
                {
                    result = await handler.Invoke(new Message<ISiteSettings>(result, this));
                }

            }

            return result;

        }

    }

}
