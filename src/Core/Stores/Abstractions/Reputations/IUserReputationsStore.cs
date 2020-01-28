using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Reputations;

namespace PlatoCore.Stores.Abstractions.Reputations
{
    public interface IUserReputationsStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<IReputation>> GetUserReputationsAsync(int userId, IEnumerable<IReputation> reputations);

    }

}
