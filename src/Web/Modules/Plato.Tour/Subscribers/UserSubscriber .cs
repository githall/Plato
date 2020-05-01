using System;
using System.Threading.Tasks;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Tour;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Subscribers
{

    public class UserSubscriber : IBrokerSubscriber
    {

        private readonly ITourDescriptorStore _tourDescriptorStore; 
        private readonly IBroker _broker;

        public UserSubscriber(
            ITourDescriptorStore tourDescriptorStore,       
            IBroker broker)
        {
            _tourDescriptorStore = tourDescriptorStore;        
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // UserUpdated
            _broker.Sub<User>(new MessageOptions()
            {
                Key = "UserUpdated"
            }, async message => await UserUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // UserUpdated
            _broker.Unsub<User>(new MessageOptions()
            {
                Key = "UserUpdated"
            }, async message => await UserUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<User> UserUpdated(User user)
        {

            if (user == null)
            {
                return user;
            }

            var stepToUpdate = DefaultSteps.UpdateProfile; ;

            // We need a step to update
            if (stepToUpdate == null)
            {        
                return user;
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
            return user;

        }

        #endregion

    }

}
