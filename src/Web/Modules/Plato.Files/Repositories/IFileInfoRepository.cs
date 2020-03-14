using System.Threading.Tasks;

namespace Plato.Files.Repositories
{

    public interface IFileInfoRepository<TModel> where TModel : class
    {

        Task<TModel> SelectByUserIdAsync(int userId);

    }

}
