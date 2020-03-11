using Plato.Attachments.Models;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Attachments.Services
{

    public interface IAttachmentValidator
    {
        Task<ICommandResult<Attachment>> ValidateAsync(Attachment attachment);
    }

}
