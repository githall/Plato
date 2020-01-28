using System.Threading.Tasks;
using PlatoCore.Models.Users;

namespace Plato.WebApi.Services
{

    public interface IWebApiAuthenticator
    {
        Task<User> GetAuthenticatedUserAsync();
    }
    
}
