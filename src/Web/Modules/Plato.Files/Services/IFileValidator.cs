using Plato.Files.Models;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Files.Services
{

    public interface IFileValidator
    {
        Task<ICommandResult<File>> ValidateAsync(File file);
    }

}
