using System.Collections.Generic;
using PlatoCore.Models.Users;

namespace Plato.Discuss.Categories.Moderators.ViewModels
{
    public class ModeratorsViewModel
    {

        public IEnumerable<User> Moderators { get; set; }

    }
}
