﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Tenants.Models;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;

namespace Plato.Tenants.Services
{

    public class TenantSetUpService : ITenantSetUpService
    {

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly ILogger<TenantSetUpService> _logger;
        private readonly IPlatoHost _platoHost;
        
        public TenantSetUpService(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            ILogger<TenantSetUpService> logger,
            IPlatoHost platoHost)
        {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;            
            _platoHost = platoHost;
            _logger = logger;
        }

        public async Task<ICommandResult<TenantSetUpContext>> InstallAsync(TenantSetUpContext context)
        {

            var result = new CommandResult<TenantSetUpContext>();

            try
            {

                // Validate tenant

                var shells = _shellSettingsManager.LoadSettings();
                if (shells != null)
                {

                    // Ensure a unique shell name
                    var shell = shells.FirstOrDefault(s => s.Name.Equals(context.SiteName, StringComparison.OrdinalIgnoreCase));
                    if (shell != null)
                    {
                        return result.Failed($"A tenant with the name \"{shell.Name}\" already exists!");
                    }

                    // Ensure a unique connection string & table prefix
                    shell = shells.FirstOrDefault(s =>
                        s.ConnectionString.Equals(context.DatabaseConnectionString, StringComparison.OrdinalIgnoreCase) &&
                        s.TablePrefix.Equals(context.DatabaseTablePrefix, StringComparison.OrdinalIgnoreCase));
                    if (shell != null)
                    {
                        return result.Failed($"A tenant with the same connection string and table prefix already exists!");
                    }

                }

                // Configure tenant

                return await InstallInternalAsync(context);

            }
            catch (Exception ex)
            {
                return result.Failed(ex.Message);
            }

        }

        public async Task<ICommandResult<TenantSetUpContext>> UpdateAsync(TenantSetUpContext context)
        {

            var result = new CommandResult<TenantSetUpContext>();

            try
            {
                return await UpdateInternalAsync(context);
            }
            catch (Exception ex)
            {
                return result.Failed(ex.Message);
            }

        }

        public async Task<ICommandResultBase> UninstallAsync(string siteName)
        {

            var result = new CommandResultBase();
            try
            {          
                return await UninstallInternalAsync(siteName);
            }
            catch (Exception ex)
            {
                return result.Failed(ex.Message);
            }

        }

        // --------------------------

        private async Task<ICommandResult<TenantSetUpContext>> InstallInternalAsync(TenantSetUpContext context)
        {

            var result = new CommandResult<TenantSetUpContext>();

            var shellSettings = BuildShellSettings(context);

            using (var shellContext = _shellContextFactory.CreateMinimalShellContext(shellSettings))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {
                    using (var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>())
                    {

                        // update dbContext confirmation
                        dbContext.Configure(options =>
                        {
                            options.ConnectionString = shellSettings.ConnectionString;
                            options.DatabaseProvider = shellSettings.DatabaseProvider;
                            options.TablePrefix = shellSettings.TablePrefix;
                        });

                        var hasErrors = false;
                        void ReportError(string key, string message)
                        {
                            hasErrors = true;
                            //result.Failed(message)
                            context.Errors[key] = message;
                        }

                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantSetUpService>>();
                        var setupEventHandlers = scope.ServiceProvider.GetServices<ISetUpEventHandler>();
                        await setupEventHandlers.InvokeAsync(x => x.SetUp(context, ReportError), logger);

                        if (hasErrors)
                        {
                            return result.Failed(context.Errors.Select(e => e.Value).ToArray());                          
                        }

                    }

                }

            }

            if (context.Errors.Count > 0)
            {
                return result.Failed(context.Errors.Select(e => e.Value).ToArray());           
            }

            shellSettings.CreatedDate = DateTimeOffset.Now;
            shellSettings.ModifiedDate = DateTimeOffset.Now;
            shellSettings.State = TenantState.Running;
            _platoHost.UpdateShellSettings(shellSettings);

            return result.Success(context);

        }

        private Task<ICommandResult<TenantSetUpContext>> UpdateInternalAsync(TenantSetUpContext context)
        {
            var result = new CommandResult<TenantSetUpContext>();
            var shellSettings = BuildShellSettings(context);
            shellSettings.ModifiedDate = DateTimeOffset.Now;
            _platoHost.UpdateShellSettings(shellSettings);
            return Task.FromResult(result.Success(context));
        }

        public const string Sql = @"
                
                -- Start Drop Tables

                DECLARE @schemaName nvarchar(100)
                DECLARE @tableName nvarchar(400)
                DECLARE @fullName nvarchar(500)
                DECLARE MSGCURSOR CURSOR FOR
                SELECT 
	                schema_name(t.schema_id) as schema_name,
	                t.name as table_name
                FROM 
	                sys.tables t
                WHERE 
	                t.name like '{prefix}%'
	
                OPEN MSGCURSOR

                FETCH NEXT FROM MSGCURSOR
                INTO @schemaName, @tableName
	
                WHILE @@FETCH_STATUS = 0
                BEGIN

	                SET @fullName = @schemaName + '.' + @tableName;
	                EXEC('DROP TABLE ' + @fullName);

	                FETCH NEXT FROM MSGCURSOR
	                INTO @schemaName, @tableName
	
                END
                -- tidy cursor
                CLOSE MSGCURSOR
                DEALLOCATE MSGCURSOR

                -- End Drop Tables

                GO
                
                -- Start Drop Procedures
               
                DECLARE @schemaName nvarchar(100)
                DECLARE @procedureName nvarchar(400)
                DECLARE @fullName nvarchar(500)
                DECLARE MSGCURSOR CURSOR FOR
                SELECT 
	                schema_name(p.schema_id) as schema_name,
	                p.name as proc_name
                FROM 
	                sys.procedures p
                WHERE 
	                p.name like 'site16_%'
	
                OPEN MSGCURSOR

                FETCH NEXT FROM MSGCURSOR
                INTO @schemaName, @procedureName
	
                WHILE @@FETCH_STATUS = 0
                BEGIN

	                SET @fullName = @schemaName + '.' + @procedureName;
	                EXEC('DROP PROCEDURE ' + @fullName);

	                FETCH NEXT FROM MSGCURSOR
	                INTO @schemaName, @procedureName
	
                END
                -- tidy cursor
                CLOSE MSGCURSOR
                DEALLOCATE MSGCURSOR
                
                -- End Drop Procedures

            ";

        private async Task<ICommandResultBase> UninstallInternalAsync(string siteName)
        {

            // Our result
            var result = new CommandResultBase();

            // Ensure the shell exists
            var shellSettings =GetShellByName(siteName);
            if (shellSettings == null)
            {
                return result.Failed($"A tenant with the name \"{siteName}\" could not be found!");
            }

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{prefix}"] = shellSettings.TablePrefix
            };

            using (var shellContext = _shellContextFactory.CreateMinimalShellContext(shellSettings))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {
                    using (var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>())
                    {

                        // update dbContext confirmation
                        dbContext.Configure(options =>
                        {
                            options.ConnectionString = shellSettings.ConnectionString;
                            options.DatabaseProvider = shellSettings.DatabaseProvider;
                            options.TablePrefix = shellSettings.TablePrefix;
                        });

                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantSetUpService>>();
                        var dbHelper = scope.ServiceProvider.GetRequiredService<IDbHelper>();

                        try
                        {
                            await dbHelper.ExecuteScalarAsync<int>(Sql, replacements);
                        }
                        catch (Exception e)
                        {
                            return result.Failed(e.Message);
                        }

                    }

                }

            }
            
            return result.Success();

        }

        private ShellSettings BuildShellSettings(TenantSetUpContext context)
        {

            var shellSettings = new ShellSettings()
            {
                Name = context.SiteName,
                Location = context.SiteName.ToSafeFileName(),
                RequestedUrlHost = context.RequestedUrlHost,
                RequestedUrlPrefix = context.RequestedUrlPrefix,
                State = context.State,
                OwnerId = context.OwnerId
            };

            if (string.IsNullOrEmpty(shellSettings.DatabaseProvider))
            {
                var tablePrefix = context.DatabaseTablePrefix;
                if (!tablePrefix.EndsWith(ShellHelper.TablePrefixSeparator))
                    tablePrefix += ShellHelper.TablePrefixSeparator;
                shellSettings.DatabaseProvider = context.DatabaseProvider;
                shellSettings.ConnectionString = context.DatabaseConnectionString;
                shellSettings.TablePrefix = tablePrefix;
            }

            shellSettings.CreatedDate = context.CreatedDate;
            shellSettings.ModifiedDate = context.ModifiedDate;

            return shellSettings;

        }

        private ShellSettings GetShellByName(string name)
        {
            var shells = _shellSettingsManager.LoadSettings();
            if (shells != null)
            {                
                var shell = shells.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (shell != null)
                {
                    return shell;
                }
            }
            return null;

        }

    }

}
