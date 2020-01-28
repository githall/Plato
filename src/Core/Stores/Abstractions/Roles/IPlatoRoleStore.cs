using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Roles;

namespace PlatoCore.Stores.Abstractions.Roles
{
    public interface IPlatoRoleStore : IStore<Role>
    {
        Task<Role> GetByNameAsync(string name);

        Task<IList<Role>> GetRolesByUserIdAsync(int userId);

        Task<IEnumerable<Role>> GetRolesAsync();
        
        Task<Role> GetByNormalizedNameAsync(string nameNormalized);

        Task<IEnumerable<string>> GetRoleNamesAsync();

        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId);

        Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId);

    }

}