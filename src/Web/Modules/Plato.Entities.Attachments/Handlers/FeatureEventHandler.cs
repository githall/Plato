using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Entities.Attachments.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";


        private readonly SchemaTable _entityyAttachments = new SchemaTable()
        {
            Name = "EntityAttachments",
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
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "AttachmentId",
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
        
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
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

                // Attachments schema
                EntityAttachments(builder);
                
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

                builder.TableBuilder.DropTable(_entityyAttachments);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityyAttachments)
                    .DropProcedure(new SchemaProcedure("SelectEntityAttachmentsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityAttachmentsByEntityId"))
                    .DropProcedure(new SchemaProcedure("DeleteEntityAttachmentByEntityIdAndAttachmentId"))
                    .DropProcedure(new SchemaProcedure("SelectEntityAttachmentsPaged"));

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

        void EntityAttachments(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_entityyAttachments);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityyAttachments)
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityAttachmentById",
                            @"SELECT 
                                    ea.*,
                                    a.FeatureId,
                                    a.[Name],
                                    a.Alias,
                                    a.Extension,
                                    CAST(1 AS BINARY(1)) AS ContentBlob, -- for perf not returned with results
                                    a.ContentType,
                                    a.ContentLength,
                                    a.ContentGuid,
                                    a.ContentCheckSum,
                                    a.TotalViews,
                                    f.ModuleId,
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
                                FROM {prefix}_Attachments a WITH (nolock) 
                                    INNER JOIN {prefix}_EntityAttachments ea ON ea.AttachmentId = a.Id                                    
                                    LEFT OUTER JOIN {prefix}_ShellFeatures f ON a.FeatureId = f.Id
                                    LEFT OUTER JOIN {prefix}_Users c ON a.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON a.ModifiedUserId = m.Id
                                WHERE (
                                   ea.Id = @Id
                                );")
                        .ForTable(_entityyAttachments)
                        .WithParameter(_entityyAttachments.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityAttachmentsByEntityId",
                            @"SELECT 
                                    ea.*,    
                                    a.FeatureId,
                                    a.[Name],
                                    a.Alias,
                                    a.Extension,
                                    CAST(1 AS BINARY(1)) AS ContentBlob, -- for perf not returned with results
                                    a.ContentType,
                                    a.ContentLength,
                                    a.ContentGuid,
                                    a.ContentCheckSum,
                                    a.TotalViews,
                                    f.ModuleId,
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
                                FROM {prefix}_Attachments a WITH (nolock) 
                                    INNER JOIN {prefix}_EntityAttachments ea ON ea.AttachmentId = a.Id                                    
                                    LEFT OUTER JOIN {prefix}_ShellFeatures f ON a.FeatureId = f.Id
                                    LEFT OUTER JOIN {prefix}_Users c ON a.CreatedUserId = c.Id
                                    LEFT OUTER JOIN {prefix}_Users m ON a.ModifiedUserId = m.Id
                                WHERE (
                                   ea.EntityId = @EntityId
                                );")
                        .ForTable(_entityyAttachments)
                        .WithParameter(new SchemaColumn() { Name = "EntityId", DbType = DbType.Int32 }))

                .CreateProcedure(new SchemaProcedure("DeleteEntityAttachmentsByEntityId", StoredProcedureType.DeleteByKey)
                    .ForTable(_entityyAttachments)
                    .WithParameter(new SchemaColumn() { Name = "EntityId", DbType = DbType.Int32 }))

                .CreateProcedure(new SchemaProcedure("DeleteEntityAttachmentByEntityIdAndAttachmentId",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(_entityyAttachments)
                    .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn() {Name = "EntityId", DbType = DbType.Int32},
                            new SchemaColumn() {Name = "AttachmentId", DbType = DbType.Int32}
                        }
                    ))

                .CreateProcedure(new SchemaProcedure("SelectEntityAttachmentsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityyAttachments)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "AttachmentId",
                            DbType = DbType.Int32,
                        },
                        new SchemaColumn()
                        {
                            Name = "EntityId",
                            DbType = DbType.Int32,
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityyAttachments.Name,
                Columns = new string[]
                {                    
                    "EntityId",
                    "AttachmentId"
                }
            });

        }

    }

}
