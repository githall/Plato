using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Files.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        private readonly SchemaTable _files = new SchemaTable()
        {
            Name = "Files",
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
                        Name = "FeatureId",
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
                        Name = "ContentGuid",
                        Length = "32",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentCheckSum",
                        Length = "32",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "TotalViews",
                        DbType = DbType.Int32
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

        private readonly IDefaultRolesManager _defaultRolesManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            IDefaultRolesManager defaultRolesManager,
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _defaultRolesManager = defaultRolesManager;
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }
        
        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");

            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Files schema
                Files(builder);

                // Attachment info schema
                FileInfo(builder);

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

        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

        public override async Task UpdatedAsync(IFeatureEventContext context)
        {
            // Apply any additional permissions to default roles for updated feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {
            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");

            using (var builder = _schemaBuilder)
            {

                builder.TableBuilder.DropTable(_files);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_files)
                    .DropProcedure(new SchemaProcedure("SelectAttachmentsPaged"))
                    .DropProcedure(new SchemaProcedure("UpdateAttachmentContentGuidById"))
                    .DropProcedure(new SchemaProcedure("SelectAttachmentInfoByUserId"));
                
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

        void Files(ISchemaBuilder builder)
        {

            // Create tables
            builder.TableBuilder.CreateTable(_files);

            // Create default procedures
            builder.ProcedureBuilder.CreateDefaultProcedures(_files);

            // Replace default SelectFileById
            builder.ProcedureBuilder
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectFileById",
                            @"SELECT 
                                    f.Id,
                                    f.FeatureId,
                                    f.[Name],
                                    f.Alias,
                                    f.Extension,
                                    f.ContentBlob,
                                    f.ContentType,
                                    f.ContentLength,
                                    f.ContentGuid,
                                    f.ContentCheckSum,
                                    f.TotalViews,
                                    f.CreatedUserId,
                                    f.CreatedDate,
                                    f.ModifiedUserId,
                                    f.ModifiedDate,
                                    sf.ModuleId,
                                    c.UserName AS CreatedUserName,
                                    c.DisplayName AS CreatedDisplayName,
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor,
                                    c.SignatureHtml AS CreatedSignatureHtml,
                                    c.IsVerified AS CreatedIsVerified,
                                    c.IsStaff AS CreatedIsStaff,
                                    c.IsSpam AS CreatedIsSpam,
                                    c.IsBanned AS CreatedIsBanned,
                                    m.UserName AS ModifiedUserName,
                                    m.DisplayName AS ModifiedDisplayName,
                                    m.Alias AS ModifiedAlias,
                                    m.PhotoUrl AS ModifiedPhotoUrl,
                                    m.PhotoColor AS ModifiedPhotoColor,
                                    m.SignatureHtml AS ModifiedSignatureHtml,
                                    m.IsVerified AS ModifiedIsVerified,
                                    m.IsStaff AS ModifiedIsStaff,
                                    m.IsSpam AS ModifiedIsSpam,
                                    m.IsBanned AS ModifiedIsBanned
                                FROM {prefix}_Files f WITH (nolock)                                     
                                    LEFT OUTER JOIN {prefix}_ShellFeatures sf ON f.FeatureId = sf.Id
                                    LEFT OUTER JOIN {prefix}_Users c ON f.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON f.ModifiedUserId = m.Id
                                WHERE (
                                   f.Id = @Id
                                );")
                        .ForTable(_files)
                        .WithParameter(_files.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectFilesPaged", StoredProcedureType.SelectPaged)
                .ForTable(_files)
                .WithParameters(new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        Name = "ContentGuid",
                        DbType = DbType.String,
                        Length = "32"
                    },
                    new SchemaColumn()
                    {
                        Name = "ContentCheckSum",
                        DbType = DbType.String,
                        Length = "32"
                    },
                    new SchemaColumn()
                    {
                        Name = "Keywords",
                        DbType = DbType.String,
                        Length = "255"
                    }
                }))

                // UpdateFileContentGuidById
                .CreateProcedure(
                    new SchemaProcedure(
                            $"UpdateFileContentGuidById",
                            @"UPDATE {prefix}_Files SET
                                    ContentGuid = @ContentGuid
                                WHERE (
                                    Id = @Id
                                );")
                        .ForTable(_files)
                        .WithParameters(new List<SchemaColumn>()
                        {
                             new SchemaColumn()
                            {
                                Name = "Id",
                                DbType = DbType.Int32,
                            },
                            new SchemaColumn()
                            {
                                Name = "ContentGuid",
                                DbType = DbType.String,
                                Length = "50"
                            }
                        }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _files.Name,
                Columns = new string[]
                {
                    "FeatureId",
                    "[Name]",
                    "Extension",
                    "ContentGuid",
                    "ContentCheckSum"
                }
            });

        }

        void FileInfo(ISchemaBuilder builder)
        {
      

            builder.ProcedureBuilder
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectFileInfoByUserId",
                            @"SELECT
	                            COUNT(f.Id) AS [Count],
	                            SUM(f.ContentLength) AS [Length]
                            FROM
	                            {prefix}_Files f
                            WHERE (
	                            f.CreatedUserId = @UserId
                            );")
                        .ForTable(_files)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32,                        
                        }));

        }

    }

}
