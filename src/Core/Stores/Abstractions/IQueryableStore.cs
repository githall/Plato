using System.Data;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Stores.Abstractions
{

    /// <summary>
    /// Represents a store that supports an IQueryBuilder.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryableStore<TModel> where TModel : class
    {

        IQuery<TModel> QueryAsync();

        Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams);

    }

}
