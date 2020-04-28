﻿using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Notifications.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        public string Version { get; } = "1.0.0";

        // EntityMentions table
        private readonly SchemaTable _userNotifications = new SchemaTable()
        {
            Name = "UserNotifications",
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
                        Name = "UserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "NotificationName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Title]",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Message]",
                        Length = "max",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        Length = "500",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ReadDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
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
        
        #region "Constructor"

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder, 
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        #endregion

        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // Configure
                Configure(builder);

                // UserNotifications schema
                UserNotifications(builder);

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

                // drop EntityMentions
                builder.TableBuilder.DropTable(_userNotifications);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_userNotifications)
                    .DropProcedure(new SchemaProcedure("SelectUserNotificationsPaged"));
                
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

        void UserNotifications(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_userNotifications);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userNotifications)

                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectUserNotificationById",
                            @" SELECT un.*, 
                                    u.UserName,
                                    u.DisplayName,
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor,
                                    c.UserName AS CreatedUserName,                                    
                                    c.DisplayName AS CreatedDisplayName,
                                    c.Alias AS CreatedAlias,
                                    c.PhotoUrl AS CreatedPhotoUrl,
                                    c.PhotoColor AS CreatedPhotoColor
                                FROM {prefix}_UserNotifications un WITH (nolock) 
                                    LEFT OUTER JOIN {prefix}_Users u ON un.UserId = u.Id
                                    LEFT OUTER JOIN {prefix}_Users c ON un.CreatedUserId = c.Id
                                WHERE (
                                   un.Id = @Id
                                )")
                        .ForTable(_userNotifications)
                        .WithParameter(_userNotifications.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectUserNotificationsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_userNotifications)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }))

                .CreateProcedure(
                    new SchemaProcedure(
                            $"UpdateUserNotificationsReadDate",
                            @"UPDATE {prefix}_UserNotifications
                    SET ReadDate = @ReadDate
                    WHERE (
                        UserId = @UserId
                    ); SELECT 1;")
                        .ForTable(_userNotifications)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "UserId",
                                DbType = DbType.Int32
                            },
                            new SchemaColumn()
                            {
                                Name = "ReadDate",
                                DbType = DbType.DateTimeOffset,
                                Nullable = true
                            }
                        }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _userNotifications.Name,
                Columns = new string[]
                {
                    "UserId",
                    "NotificationName",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });

        }

        #endregion
        
    }

}
