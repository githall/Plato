using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Models.Schema;
using PlatoCore.Stores.Abstractions.Schema;

namespace PlatoCore.Stores.Extensions
{
    public static class ConstraintStoreExtensions
    {

        public static async Task<DbConstraint> GetPrimaryKeyConstraint(
            this IConstraintStore store,
            string tableName)
        {
            var constraints = await store.SelectConstraintsAsync();
            return constraints.FirstOrDefault(c => c.TableName == tableName && c.ConstraintType == ConstraintTypes.PrimaryKey);
        }

    }
}
