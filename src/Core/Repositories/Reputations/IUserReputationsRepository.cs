using PlatoCore.Repositories;

namespace PlatoCore.Repositories.Reputations
{
    public interface IUserReputationsRepository<T> : IRepository<T> where T : class
    {
    }
}
