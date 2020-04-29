using PlatoCore.Repositories;

namespace Plato.Site.Repositories
{

    public interface ISignUpRepository<T> : IRepository<T> where T : class
    {
    }

}
