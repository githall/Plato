using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Entities.Files.Stores
{
    public interface IEntityFileStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByFileIdAsync(int fileId);

        Task<bool> DeleteByEntityIdAndFileIdAsync(int entityId, int LabelId);

    }

}
