using PlatoCore.Repositories;
using System.Threading.Tasks;

namespace Plato.Files.Repositories
{

    public interface IFileRepository<T> : IRepository<T> where T : class
    {

        Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid);

        Task<bool> UpdateContentGuidAsync(int id, string contentGuid);

    }

}
