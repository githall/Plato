using PlatoCore.Repositories;
using System.Threading.Tasks;

namespace Plato.Tenants.SignUp.Repositories
{

    public interface ISignUpRepository<T> : IRepository<T> where T : class
    {

        Task<T> SelectBySessionIdAsync(string sessionId);

    }

}
