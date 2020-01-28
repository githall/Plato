using Plato.Entities.Models;
using PlatoCore.Security.Abstractions;

namespace Plato.Entities.Ratings.ViewModels
{
    public class VoteToggleViewModel
    {

        public IEntity Entity { get; set; }

        public IEntityReply Reply { get; set; }

        public string ApiUrl { get; set; }

        public IPermission Permission { get; set; }

    }

}
