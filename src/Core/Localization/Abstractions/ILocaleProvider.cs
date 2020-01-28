using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Localization.Abstractions
{
    public interface ILocaleProvider
    {
        Task<IEnumerable<ComposedLocaleDescriptor>> GetLocalesAsync();

        void Dispose();

    }

}
