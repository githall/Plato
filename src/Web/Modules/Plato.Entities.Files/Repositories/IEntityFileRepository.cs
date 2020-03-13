using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Entities.Files.Repositories
{
    public interface IEntityFileRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndFileIdAsync(int entityId, int fileId);

    }


}
