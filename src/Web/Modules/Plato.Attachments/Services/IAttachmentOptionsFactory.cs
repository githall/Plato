using Plato.Attachments.Models;
using PlatoCore.Models.Users;
using System.Threading.Tasks;

namespace Plato.Attachments.Services
{

    public interface IAttachmentOptionsFactory
    {
        Task<AttachmentOptions> GetOptionsAsync(IUser user);
    }

}
