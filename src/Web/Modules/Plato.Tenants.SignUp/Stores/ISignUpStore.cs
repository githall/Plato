using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Tenants.SignUp.Stores
{

    public interface ISignUpStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<TModel> GetBySessionIdAsync(string sessionId);

    }

}
