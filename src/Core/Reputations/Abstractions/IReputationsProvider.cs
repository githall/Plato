using System.Collections.Generic;
using PlatoCore.Models.Reputations;

namespace PlatoCore.Reputations.Abstractions
{
    public interface IReputationsProvider<out TReputation> where TReputation : class, IReputation
    {
        IEnumerable<TReputation> GetReputations();
    }
}
