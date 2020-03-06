using Plato.Attachments.Models;
using System.Threading.Tasks;

namespace Plato.Attachments.Services
{

    public interface IAttachmentOptionsFactory
    {
        Task<AttachmentOptions> GetSettingsAsync();
    }

}
