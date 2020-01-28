using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Reputations.Abstractions
{
    public interface IReputationsManager<TReputation> where TReputation : class
    {
        Task<IEnumerable<TReputation>> GetReputationsAsync();

        Task<IDictionary<string, IEnumerable<TReputation>>> GetCategorizedReputationsAsync();

    }

}
