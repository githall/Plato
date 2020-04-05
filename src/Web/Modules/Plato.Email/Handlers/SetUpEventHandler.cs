using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Email.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        public string Version { get; } = "1.0.0";

        // Email table
        private readonly SchemaTable _emails = new SchemaTable()
        {
            Name = "Emails",
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
                        Name = "[To]",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Cc",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Bcc",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[From]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Subject",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Body]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Priority",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "SendAttempts",
                        DbType = DbType.Int16
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

        // Email attachments table
        private readonly SchemaTable _emailAttachments = new SchemaTable()
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

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        public override async Task SetUp(SetUpContext context, Action<string, string> reportError)
        {

            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Emails schema
                Emails(builder);

                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    reportError(error, $"SetUp within {this.GetType().FullName} - {error}");
                }

            }

        }

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Emails(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_emails);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_emails)
                .CreateProcedure(new SchemaProcedure("SelectEmailsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_emails)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }

        void EmailAttachments(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_emailAttachments);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_emailAttachments)
                .CreateProcedure(new SchemaProcedure("SelectEmailAttachmentsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_emailAttachments)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

        }

        #endregion

    }

}
