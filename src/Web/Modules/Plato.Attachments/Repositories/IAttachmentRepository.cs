using PlatoCore.Repositories;
using System.Threading.Tasks;

namespace Plato.Attachments.Repositories
{

    public interface IAttachmentRepository<T> : IRepository<T> where T : class
    {

        Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid);

        Task<bool> UpdateContentGuidAsync(int id, string contentGuid);

    }

}
