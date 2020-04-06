using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Files.Sharing.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // File invites table
        private readonly SchemaTable _fileInvites = new SchemaTable()
        {
            Name = "FileInvites",
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
                        Name = "FileId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        DbType = DbType.String,
                        Length = "255"
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

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");

            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // File invites
                FileInvites(builder);

                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
                }

            }

        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {
            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");

            using (var builder = _schemaBuilder)
            {

                builder.TableBuilder.DropTable(_fileInvites);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_fileInvites)
                    .DropProcedure(new SchemaProcedure("SelectFileInvitesPaged"))
                    .DropProcedure(new SchemaProcedure("SelectFileInvitesByEmailAndFileId"))
                    .DropProcedure(new SchemaProcedure("SelectFileInviteByEmailFileIdAndCreatedUserId"));

                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Logger.LogCritical(error, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                    context.Errors.Add(error, $"UninstallingAsync within {this.GetType().FullName}");
                }

            }

        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        #endregion

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

        void FileInvites(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_fileInvites);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_fileInvites)
            
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectFileInviteById",
                            @"SELECT fi.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_FileInvites fi WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON fi.CreatedUserId = u.Id 
                                WHERE (
                                    fi.Id = @Id 
                                )")
                        .ForTable(_fileInvites)
                        .WithParameter(_fileInvites.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure("SelectFileInvitesByEmailAndFileId",
                            @"SELECT fi.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_FileInvites fi WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON fi.CreatedUserId = u.Id 
                                WHERE (
                                    (fi.Email = @Email AND fi.FileId = @FileId) AND
                                    (u.EmailConfirmed = 1 AND u.LockoutEnabled = 0)
                                )")
                        .ForTable(_fileInvites)
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
                                Name = "FileId",
                                DbType = DbType.Int32,
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectFileInviteByEmailFileIdAndCreatedUserId",
                            @"SELECT fi.*, 
                                u.Email, 
                                u.UserName, 
                                u.DisplayName, 
                                u.NormalizedUserName 
                                FROM {prefix}_FileInvites fi WITH (nolock) 
                                LEFT OUTER JOIN {prefix}_Users u ON fi.CreatedUserId = u.Id 
                                WHERE (
                                    fi.Email = @Email AND fi.FileId = @FileId AND u.Id = @CreatedUserId
                                )")
                        .ForTable(_fileInvites)
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
                                Name = "FileId",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "CreatedUserId",
                                DbType = DbType.Int32,
                            }
                        }))

                .CreateProcedure(new SchemaProcedure("SelectFileInvitesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_fileInvites)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Email",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _fileInvites.Name,
                Columns = new string[]
                {
                    "Email",
                    "FileId",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });

        }

        #endregion

    }

}
