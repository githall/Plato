using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace Plato.Site.Services
{
    public interface ISignUpValidator
    {

        Task<ICommandResultBase> ValidateEmailAsync(string email);

        Task<ICommandResultBase> ValidateCompanyNameAsync(string companyName);

    }

}
