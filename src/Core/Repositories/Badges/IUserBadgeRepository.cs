using PlatoCore.Repositories;

namespace PlatoCore.Repositories.Badges
{
    public interface IUserBadgeRepository<T> : IRepository<T> where T : class
    {
    }
}
