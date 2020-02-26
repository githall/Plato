using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Entities.Attachments.Stores
{
    public interface IEntityAttachmentStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndLabelIdAsync(int entityId, int LabelId);

    }

}
