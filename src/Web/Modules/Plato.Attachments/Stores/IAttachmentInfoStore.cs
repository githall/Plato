using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Attachments.Stores
{
    public interface IAttachmentInfoStore<TModel> : ICacheableStore<TModel> where TModel : class
    {
  
        Task<TModel> GetByUserIdAsync(int userId);

    }

}
