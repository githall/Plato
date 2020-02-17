using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlatoCore.Data.Schemas.Abstractions
{

    public interface ISchemaManager
    {
        Task<IEnumerable<string>> ExecuteAsync(IEnumerable<string> statements);
    }

}
