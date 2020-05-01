using PlatoCore.Models.Tour;
using System.Threading.Tasks;

namespace PlatoCore.Stores.Abstractions.Tour
{
    public interface ITourDescriptorStore : ISettingsStore<TourDescriptor>
    {

        Task<TourStep> GetStepAsync(string id);

        Task<TourDescriptor> UpdateStepAsync(TourStep step);

    }

}
