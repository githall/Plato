using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Tour;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Subscribers
{

    public class EmailSettingsSubscriber : IBrokerSubscriber
    {

        private readonly ITourDescriptorStore _tourDescriptorStore; 
        private readonly IBroker _broker;

        public EmailSettingsSubscriber(
            ITourDescriptorStore tourDescriptorStore,       
            IBroker broker)
        {
            _tourDescriptorStore = tourDescriptorStore;        
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // EmailSettingsUpdated
            _broker.Sub<EmailSettings>(new MessageOptions()
            {
                Key = "EmailSettingsUpdated"
            }, async message => await EmailSettingsUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // EmailSettingsUpdated
            _broker.Unsub<EmailSettings>(new MessageOptions()
            {
                Key = "EmailSettingsUpdated"
            }, async message => await EmailSettingsUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EmailSettings> EmailSettingsUpdated(EmailSettings settings)
        {

            if (settings == null)
            {
                return settings;
            }

            var stepToUpdate = DefaultSteps.EmailSettings; ;

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
