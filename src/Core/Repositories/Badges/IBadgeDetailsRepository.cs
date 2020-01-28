using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Badges;

namespace PlatoCore.Repositories.Badges
{

    public interface IBadgeDetailsRepository 
    {
        Task<IEnumerable<BadgeDetails>> SelectAsync();

        Task<IEnumerable<BadgeDetails>> SelectByUserIdAsync(int userId);
    }

}
