using System.Threading.Tasks;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Subscribers
{

    public class FeatureSubscriber : IBrokerSubscriber
    {

        private readonly ITourDescriptorStore _tourDescriptorStore; 
        private readonly IBroker _broker;

        public FeatureSubscriber(
            ITourDescriptorStore tourDescriptorStore,       
            IBroker broker)
        {
            _tourDescriptorStore = tourDescriptorStore;        
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Installed
            _broker.Sub<IShellFeature>(new MessageOptions()
            {
                Key = "FeatureInstalled"
            }, async message => await FeatureInstalled(message.What));

        }

        public void Unsubscribe()
        {
            // Installed
            _broker.Unsub<IShellFeature>(new MessageOptions()
            {
                Key = "FeatureInstalled"
            }, async message => await FeatureInstalled(message.What));

        }

        #endregion

        #region "Private Methods"

        Task<IShellFeature> FeatureInstalled(IShellFeature feature)
        {
            // Return
            return Task.FromResult(feature);
        }

        #endregion

    }

}
