using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Files.Stores
{
    public interface IFileInfoStore<TModel> : ICacheableStore<TModel> where TModel : class
    {
  
        Task<TModel> GetByUserIdAsync(int userId);

    }

}
