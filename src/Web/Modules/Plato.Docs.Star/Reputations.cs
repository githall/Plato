using System.Collections.Generic;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Docs.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarDoc =
            new Reputation("Star Doc", 1);

        public static readonly Reputation StarredDoc =
            new Reputation("Starred Doc", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarDoc,
                StarredDoc
            };
        }

    }

}
