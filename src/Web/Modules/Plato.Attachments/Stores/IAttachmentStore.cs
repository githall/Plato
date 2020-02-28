using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Attachments.Stores
{
    public interface IAttachmentStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid);

        Task<bool> UpdateContentGuidAsync(int id, string contentGuid);

    }

}
