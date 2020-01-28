using System.Threading.Tasks;

namespace PlatoCore.Stores.Abstractions.Users
{
    public interface IUserBannerStore<T> : IStore<T> where T : class
    {
        Task<T> GetByUserIdAsync(int userId);
    }
}