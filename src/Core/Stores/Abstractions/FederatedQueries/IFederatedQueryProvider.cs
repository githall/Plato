using System.Collections.Generic;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Stores.Abstractions.FederatedQueries
{
    public interface IFederatedQueryProvider<TModel> where TModel : class
    {
        IEnumerable<string> Build(IQuery<TModel> query);
    }
}
