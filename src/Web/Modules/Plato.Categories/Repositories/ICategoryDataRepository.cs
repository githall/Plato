using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Categories.Repositories
{
    public interface ICategoryDataRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectByCategoryIdAsync(int userId);

    }

}
