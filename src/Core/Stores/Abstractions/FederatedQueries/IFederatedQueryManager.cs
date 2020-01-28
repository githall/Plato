using System.Collections.Generic;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Stores.Abstractions.FederatedQueries
{

    public interface IFederatedQueryManager<TModel> where TModel : class
    {
        IEnumerable<string> GetQueries(IQuery<TModel> query);
    }
    
}
