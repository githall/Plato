using System;
using System.Threading.Tasks;
using Plato.Tour.Models;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Models.Tour;
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

        async Task<IShellFeature> FeatureInstalled(IShellFeature feature)
        {

            if (feature == null)
            {
                return feature;
            }

            TourStep stepToUpdate = null;

            if (ShellDescriptors.CoreModules.Contains(feature.Descriptor.Id))
            {
                stepToUpdate = DefaultSteps.EnablleCoreFeature;
            }

            if (ShellDescriptors.OptionalModules.Contains(feature.Descriptor.Id))
            {
                stepToUpdate = DefaultSteps.EnablleOptionalFeature;
            }

            if (ShellDescriptors.SearchModules.Contains(feature.Descriptor.Id))
            {
                stepToUpdate = DefaultSteps.EnableSearch;
            }

            // We need a step to update
            if (stepToUpdate == null)
            {        
                return feature;
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
            return feature;

        }

        #endregion

    }

}
