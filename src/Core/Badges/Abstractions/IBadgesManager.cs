using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Badges.Abstractions
{
    public interface IBadgesManager<TBadge> where TBadge : class
    {
        IEnumerable<TBadge> GetBadges();

        Task<IDictionary<string, IEnumerable<TBadge>>> GetCategorizedBadgesAsync();

    }

}
