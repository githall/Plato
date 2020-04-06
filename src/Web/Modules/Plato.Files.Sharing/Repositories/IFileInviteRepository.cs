using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Repositories;

namespace Plato.Files.Sharing.Repositories
{
    public interface IFileInviteRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByEmailAndFileIdAsync(string email, int fileId);

        Task<TModel> SelectByEmailFileIdAndCreatedUserIdAsync(string email, int fileId, int userId);

    }

}
