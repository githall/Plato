using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Security.Abstractions
{
    
    public interface IPermissionsManager<TPermission> where TPermission : class
    {

        IEnumerable<TPermission> GetPermissions();

        Task<IDictionary<string, IEnumerable<TPermission>>> GetCategorizedPermissionsAsync();
        
    }

}
