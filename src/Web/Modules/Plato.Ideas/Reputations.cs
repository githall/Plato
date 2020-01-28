using System.Collections.Generic;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Ideas
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewIdea =
            new Reputation("New Idea", 1);

        public static readonly Reputation NewIdeaComment =
            new Reputation("New Suggestion", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewIdea,
                NewIdeaComment
            };
        }

    }
}
