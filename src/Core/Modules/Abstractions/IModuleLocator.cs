using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Modules;

namespace PlatoCore.Modules.Abstractions
{
    public interface IModuleLocator
    {
        Task<IEnumerable<IModuleDescriptor>> LocateModulesAsync(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
