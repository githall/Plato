using Plato.Files.Models;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Files.Services
{

    public interface IAttachmentValidator
    {
        Task<ICommandResult<File>> ValidateAsync(File attachment);
    }

}
