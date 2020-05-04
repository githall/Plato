using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Tenants.SignUp.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.1";


        private readonly SchemaTable _signUps = new SchemaTable()
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
                        Name = "SessionId",
                        Length = "255",
                        DbType = DbType.String
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

                // SignUps schema
                SignUps(builder);
                
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

                builder.TableBuilder.DropTable(_signUps);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_signUps)
                    .DropProcedure(new SchemaProcedure("SelectSignUpsPaged", StoredProcedureType.SelectByKey))
                    .DropProcedure(new SchemaProcedure("SelectSignUpBySessionId", StoredProcedureType.SelectByKey));                 

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

        void SignUps(ISchemaBuilder builder)
        {
            
            builder.TableBuilder.CreateTable(_signUps);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_signUps)
                .CreateProcedure(new SchemaProcedure("SelectSignUpsPaged", StoredProcedureType.SelectPaged)
                .ForTable(_signUps)
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

            builder.ProcedureBuilder.CreateProcedure(new SchemaProcedure("SelectSignUpBySessionId", StoredProcedureType.SelectByKey)
                    .ForTable(_signUps)
                    .WithParameter(new SchemaColumn() { Name = "SessionId", DbType = DbType.String, Length = "255" }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _signUps.Name,
                Columns = new string[]
                {
                    "Id",
                    "Email",
                    "CompanyName",
                    "CompanyNameAlias"
                }
            });


        }

    }

}
