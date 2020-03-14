using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Files.Stores
{
    public interface IFileStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid);

        Task<bool> UpdateContentGuidAsync(int id, string contentGuid);

    }

}
