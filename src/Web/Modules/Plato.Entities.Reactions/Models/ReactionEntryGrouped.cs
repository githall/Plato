using System.Collections.Generic;
using PlatoCore.Models.Users;

namespace Plato.Entities.Reactions.Models
{

    public class ReactionEntryGrouped : ReactionEntry
    {

        public ReactionEntryGrouped(IReactionEntry reaction) : base(reaction)
        {
            CreatedBy = reaction.CreatedBy;
            CreatedDate = reaction.CreatedDate;
            this.Users.Add(reaction.CreatedBy);
        }
        
        public IList<ISimpleUser> Users { get; } = new List<ISimpleUser>();

    }

}
