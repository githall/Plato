using System.Data;
using System.Collections.Generic;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Site
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

            // Plato.Site 1.0.1 add a new SignUps table and set of stored procedures

             var signUps = new SchemaTable()
             {
                 Name = "SignUps",
                 Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CompanyName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CompanyNameAlias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailUpdates",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityToken",
                        Length = "8",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
             };

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.1";
                    });

                // ---------------
                // SignUps
                // ---------------

                builder.TableBuilder.CreateTable(signUps);

                builder.ProcedureBuilder
                    .CreateDefaultProcedures(signUps)
                    .CreateProcedure(new SchemaProcedure("SelectSignUpPaged", StoredProcedureType.SelectPaged)
                    .ForTable(signUps)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Email",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "CompanyName",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "CompanyNameAlias",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

                // Indexes
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = signUps.Name,
                    Columns = new string[]
                    {
                        "Id",
                        "Email",
                        "CompanyName",
                        "CompanyNameAlias"
                    }
                });

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
