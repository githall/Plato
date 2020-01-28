using System.Collections.Generic;
using System.Text;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Stores.Abstractions.QueryAdapters
{

    public interface IQueryAdapterManager<TModel> where TModel : class
    {

        void BuildSelect(IQuery<TModel> query, StringBuilder builder);

        void BuildTables(IQuery<TModel> query, StringBuilder builder);

        void BuildWhere(IQuery<TModel> query, StringBuilder builder);

    }
    
}
