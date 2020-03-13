using Plato.Files.Models;
using PlatoCore.Models.Users;
using System.Threading.Tasks;

namespace Plato.Files.Services
{

    public interface IAttachmentOptionsFactory
    {
        Task<FileOptions> GetOptionsAsync(IUser user);
    }

}
