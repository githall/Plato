using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PlatoCore.Data.Migrations.Abstractions
{
    public abstract class BaseMigrationProvider : IMigrationProvider
    {

        public IList<PreparedMigration> AvailableMigrations { get; set; }

        public string ModuleId => Path.GetFileNameWithoutExtension(this.GetType().Assembly.ManifestModule.Name);

        public IList<PreparedMigration> Schemas { get; } = new List<PreparedMigration>();

        public virtual PreparedMigration GetSchema(string version)
        {
            return AvailableMigrations.FirstOrDefault(s => s.Version.Equals(version, StringComparison.OrdinalIgnoreCase));
        }

        public virtual IMigrationProvider LoadSchemas(IList<string> versions)
        {

            foreach (var migration in AvailableMigrations)
            {
                if (versions.Contains(migration.Version))
                {
                    Schemas.Add(migration);
                }
            }

            return this;

        }

    }

}
