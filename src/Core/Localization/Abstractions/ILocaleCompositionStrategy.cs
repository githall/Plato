using System.Threading.Tasks;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Localization.Abstractions
{

    public interface ILocaleCompositionStrategy
    {

        Task<ComposedLocaleDescriptor> ComposeLocaleDescriptorAsync(LocaleDescriptor descriptor);

    }

}
