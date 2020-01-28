using System.Threading.Tasks;

namespace PlatoCore.Data.Migrations.Abstractions
{

    public interface IDataMigrationManager
    {
        Task<DataMigrationResult> ApplyMigrationsAsync(DataMigrationRecord dataMigrationRecord);
    }

}
