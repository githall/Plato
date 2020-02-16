using System.Collections.Generic;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Roles
{

    public class Migrations : BaseMigrationProvider
    {

        private readonly ISchemaBuilder _schemaBuilder;

        public Migrations(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;

            AvailableMigrations = new List<PreparedMigration>
            {
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.1",
                    Statements = V_1_0_1()
                }
            };

        }

        // Migrations

        private ICollection<string> V_1_0_1()
        {

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.1";
                    });

                // Roles
                builder.IndexBuilder
                    .CreateIndex(new SchemaIndex()
                        {
                            TableName = "Roles",
                            Columns = new string[]
                            {
                                "[Name]",
                                "NormalizedName"
                            }
                        })

                        // UserRoles
                        .CreateIndex(new SchemaIndex()
                        {
                            TableName = "UserRoles",
                            Columns = new string[]
                            {
                                "UserId",
                                "RoleId"
                            }
                        });

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
