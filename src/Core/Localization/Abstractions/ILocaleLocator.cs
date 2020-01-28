using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Localization.Abstractions
{
    public interface ILocaleLocator
    {
        Task<IEnumerable<LocaleDescriptor>> LocateLocalesAsync(IEnumerable<string> paths);

    }


}
