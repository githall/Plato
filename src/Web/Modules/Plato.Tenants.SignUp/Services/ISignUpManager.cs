using PlatoCore.Abstractions;

namespace Plato.Tenants.SignUp.Services
{

    public interface ISignUpManager<TModel> : ICommandManager<TModel> where TModel : class
    {
    }

}
