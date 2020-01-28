using System;
using PlatoCore.Models.Users;

namespace Plato.Entities.Reactions.Models
{

    public interface IReactionEntry : IReaction
    {
        ISimpleUser CreatedBy { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

    }

}
