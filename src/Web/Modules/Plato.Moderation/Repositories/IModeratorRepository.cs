using PlatoCore.Repositories;

namespace Plato.Moderation.Repositories
{
    public interface IModeratorRepository<T> : IRepository<T> where T : class
    {

    }

}
