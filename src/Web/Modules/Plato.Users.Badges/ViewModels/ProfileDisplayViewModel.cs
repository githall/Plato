using System.Collections.Generic;
using PlatoCore.Models.Badges;
using PlatoCore.Models.Users;

namespace Plato.Users.Badges.ViewModels
{
    public class ProfileDisplayViewModel
    {
        public User User { get; set; }

        public IEnumerable<IBadgeEntry> Badges { get; set; }

    }
}
