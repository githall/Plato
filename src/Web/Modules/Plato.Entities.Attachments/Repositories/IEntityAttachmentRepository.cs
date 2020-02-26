using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Entities.Attachments.Repositories
{
    public interface IEntityAttachmentRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndAttachmentIdAsync(int entityId, int attachmentId);

    }


}
