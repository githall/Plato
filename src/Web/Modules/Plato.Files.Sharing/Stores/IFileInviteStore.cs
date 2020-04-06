using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Stores.Abstractions;

namespace Plato.Files.Sharing.Stores
{
    public interface IFileInviteStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByEmailAndFileIdAsync(string email, int fileId);

        Task<TModel> SelectByEmailFileIdAndCreatedUserIdAsync(string email, int fileId, int createdUserId);

    }


}
