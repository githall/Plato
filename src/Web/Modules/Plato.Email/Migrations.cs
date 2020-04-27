using System.Data;
using System.Collections.Generic;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Email
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
                    Statements = v_1_0_1()
                }
            };

        }
        
        private ICollection<string> v_1_0_1()
        {

            // Plato.Emails 1.0.1 adds support for email attachments
            // ----------------

            var emailAttachments = new SchemaTable()
            {
                Name = "EmailAttachments",
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
                        Name = "EmailId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "[Name]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Extension",
                        Length = "16",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentBlob",
                        Nullable = true,
                        DbType = DbType.Binary
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentType",
                        Length = "75",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentLength",
                        DbType = DbType.Int64
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentCheckSum",
                        Length = "32",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
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
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    });

                builder.TableBuilder.CreateTable(emailAttachments);

                builder.ProcedureBuilder
                    .CreateDefaultProcedures(emailAttachments)
                    .CreateProcedure(new SchemaProcedure("SelectEmailAttachmentsPaged", StoredProcedureType.SelectPaged)
                        .ForTable(emailAttachments)
                        .WithParameters(new List<SchemaColumn>()
                        {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                        }))

                    // Returns all attachments for a specific email
                    .CreateProcedure(
                        new SchemaProcedure("SelectEmailAttachmentsByEmailId",
                                @"SELECT *
                                FROM {prefix}_EmailAttachments WITH (nolock)                                     
                                WHERE (
                                   EmailId = @EmailId
                                )")
                            .ForTable(emailAttachments)
                            .WithParameter(new SchemaColumn()
                            {
                                Name = "EmailId",
                                DbType = DbType.Int32
                            }));

                // Indexes
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = emailAttachments.Name,
                    Columns = new string[]
                    {
                        "EmailId"
                    }
                });

                // Add builder results to output
                output.AddRange(builder.Statements);
                
            }

            return output;

        }

    }

}
