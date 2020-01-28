using System.Threading.Tasks;

namespace PlatoCore.Repositories.Users
{
    public interface IUserPhotoRepository<T> : IRepository<T> where T : class
    {

        Task<T> SelectByUserIdAsync(int userId);

    }
}
