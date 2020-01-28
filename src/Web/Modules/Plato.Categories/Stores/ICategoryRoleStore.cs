using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface ICategoryRoleStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByCategoryIdAsync(int categoryId);

        Task<bool> DeleteByCategoryIdAsync(int categoryId);
        
        Task<bool> DeleteByRoleIdAndCategoryIdAsync(int roleId, int categoryId);

    }


}
