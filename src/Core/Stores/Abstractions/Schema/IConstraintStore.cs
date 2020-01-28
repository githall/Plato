using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Schema;

namespace PlatoCore.Stores.Abstractions.Schema
{
    public interface IConstraintStore
    {

        Task<IEnumerable<DbConstraint>> SelectConstraintsAsync();

    }

}
