using System.Collections.Generic;
using PlatoCore.Models.Badges;

namespace PlatoCore.Badges.Abstractions
{
    public interface IBadgesProvider<out TBadge> where TBadge : class, IBadge
    {
        IEnumerable<TBadge> GetBadges();
    }

}
