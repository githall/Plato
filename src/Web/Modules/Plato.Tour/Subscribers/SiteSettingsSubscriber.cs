using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Tour;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Subscribers
{

    public class SiteSettingsSubscriber : IBrokerSubscriber
    {

        private readonly ITourDescriptorStore _tourDescriptorStore; 
        private readonly IBroker _broker;

        public SiteSettingsSubscriber(
            ITourDescriptorStore tourDescriptorStore,       
            IBroker broker)
        {
            _tourDescriptorStore = tourDescriptorStore;        
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // SiteSettingsUpdated
            _broker.Sub<ISiteSettings>(new MessageOptions()
            {
                Key = "SiteSettingsUpdated"
            }, async message => await SiteSettingsUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // SiteSettingsUpdated
            _broker.Unsub<ISiteSettings>(new MessageOptions()
            {
                Key = "SiteSettingsUpdated"
            }, async message => await SiteSettingsUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<ISiteSettings> SiteSettingsUpdated(ISiteSettings settings)
        {

            if (settings == null)
            {
                return settings;
            }

            var stepToUpdate = DefaultSteps.GeneralSettings; ;

            // We need a step to update
            if (stepToUpdate == null)
            {        
                return settings;
            }

            // Update step
            var step = await _tourDescriptorStore.GetStepAsync(stepToUpdate.Id);
            if (step != null)
            {
                if (!step.CompletedDate.HasValue)
                {
                    step.CompletedDate = DateTimeOffset.Now;
                    await _tourDescriptorStore.UpdateStepAsync(step);
                }        
            }

            // Return
            return settings;

        }

        #endregion

    }

}
