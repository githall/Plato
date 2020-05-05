using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace Plato.Tenants.SignUp.Services
{
    public interface ISignUpValidator
    {

        Task<ICommandResultBase> ValidateEmailAsync(string email);

        Task<ICommandResultBase> ValidateCompanyNameAsync(string companyName);

    }

}
