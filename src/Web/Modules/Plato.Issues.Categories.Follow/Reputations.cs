using System.Collections.Generic;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Issues.Categories.Follow
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewFollow =
            new Reputation("Issue Category Follow", 1);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewFollow
            };
        }

    }
}
