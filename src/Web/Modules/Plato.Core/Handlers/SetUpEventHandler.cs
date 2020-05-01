﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Core.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        public const string Version = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        public override async Task SetUp(
            ISetUpContext context,
            Action<string, string> reportError)
        {

            // --------------------------
            // Build core schema
            // --------------------------

            var dictionaryTable = new SchemaTable()
            {
                Name = "DictionaryEntries",
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
                        Name = "[Key]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
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
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    }
                }
            };

            var documentTable = new SchemaTable()
            {
                Name = "DocumentEntries",
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
                        Name = "[Type]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Value]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedDate",
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "ModifiedUserId",
                        DbType = DbType.Int32,
                        Nullable = true
                    }
                }
            };

            using (var builder = _schemaBuilder)
            {

                // Build dictionary store
                // --------------------------

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Core";
                        options.Version = Version;
                    });
                builder.TableBuilder.CreateTable(dictionaryTable);

                builder.ProcedureBuilder.CreateDefaultProcedures(dictionaryTable)
                    .CreateProcedure(
                        new SchemaProcedure("SelectDictionaryEntryByKey", StoredProcedureType.SelectByKey)
                            .ForTable(dictionaryTable).WithParameter(new SchemaColumn()
                            {
                                Name = "[Key]",
                                Length = "255",
                                DbType = DbType.String
                            }));

                builder.ProcedureBuilder.CreateDefaultProcedures(dictionaryTable)
                    .CreateProcedure(
                        new SchemaProcedure("DeleteDictionaryEntryByKey", StoredProcedureType.DeleteByKey)
                            .ForTable(dictionaryTable).WithParameter(new SchemaColumn()
                            {
                                Name = "[Key]",
                                Length = "255",
                                DbType = DbType.String
                            }));

                // Build document store
                // --------------------------

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = "Plato.Core";
                        options.Version = Version;
                    });

                builder.TableBuilder.CreateTable(documentTable);

                builder.ProcedureBuilder
                    .CreateDefaultProcedures(documentTable)
                    .CreateProcedure(
                        new SchemaProcedure("SelectDocumentEntryByType", StoredProcedureType.SelectByKey)
                            .ForTable(documentTable)
                            .WithParameter(new SchemaColumn()
                            {
                                Name = "[Type]",
                                Length = "500",
                                DbType = DbType.String
                            }));

                // Did any errors occur?

                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    reportError(error, $"SetUp within {this.GetType().FullName} - {error}");
                }

            }

        }

    }

}
