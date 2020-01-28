using System.Collections.Generic;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Articles
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewArticle =
            new Reputation("New Article", 1);

        public static readonly Reputation NewComment =
            new Reputation("New Comment", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewArticle,
                NewComment
            };
        }

    }
}
