using System.Data;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Repositories
{

    /// <summary>
    /// Represents a repository that supports an IPagedResults.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryableRepository<TModel> where TModel : class
    {
        Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams);

    }

}
