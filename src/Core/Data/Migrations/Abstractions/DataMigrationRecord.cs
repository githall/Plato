using System.Collections.Generic;

namespace PlatoCore.Data.Migrations.Abstractions
{

    public class DataMigrationRecord
    {

        public DataMigrationRecord()
        {
            Migrations = new List<DataMigration>();
        }

        public List<DataMigration> Migrations { get; set; }

    }

}
