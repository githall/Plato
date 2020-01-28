using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using PlatoCore.Models.Modules;

namespace PlatoCore.Modules.Abstractions
{
    public interface IModuleManager
    {
        
        Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync();

        Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync(string[] moduleIds);
        
        Task<IEnumerable<IModuleEntry>> LoadModulesAsync();

        Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds);

    }
}
