using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Schema;

namespace PlatoCore.Repositories.Schema
{
    public interface IConstraintRepository
    {
        Task<IEnumerable<DbConstraint>> SelectConstraintsAsync();
    }

}
