using System.Text;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Stores.Abstractions.QueryAdapters
{

    /// <summary>
    /// Provides extensibility into IQuery implementations.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryAdapterProvider<TModel> where TModel : class
    {
        void BuildSelect(IQuery<TModel> query, StringBuilder builder);

        void BuildTables(IQuery<TModel> query, StringBuilder builder);

        void BuildWhere(IQuery<TModel> query, StringBuilder builder);

    }
}
