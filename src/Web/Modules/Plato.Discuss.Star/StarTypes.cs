using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Discuss.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Topic =
            new StarType(
                "Topic",
                "Star",
                "Star this topic",
                "Unstar",
                "Delete star",
                "Login to star this topic",
                "You don't have permission to star topics");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Topic
            };
        }

    }

}
