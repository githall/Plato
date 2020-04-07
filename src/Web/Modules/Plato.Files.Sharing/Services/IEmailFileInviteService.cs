using Plato.Files.Sharing.Models;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Files.Sharing.Services
{
    public interface IEmailFileInviteService
    {
        Task<ICommandResult<FileInvite>> SendAttachmentInviteAsync(FileInvite invite);

        Task<ICommandResult<FileInvite>> SendLinkInviteAsync(FileInvite invite);

    }

}
