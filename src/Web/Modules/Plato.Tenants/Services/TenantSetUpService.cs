using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using Plato.Tenants.Models;
using PlatoCore.Hosting.Abstractions;
using Plato.Email.Services;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Tenants.Services
{

    public class TenantSetUpService : ITenantSetUpService
    {

        public const string UninstallSql = @"
                
                -- Drop Tables

                DECLARE @schemaName NVARCHAR(100)
                DECLARE @tableName NVARCHAR(400)
                DECLARE @procedureName NVARCHAR(400)
                DECLARE @fullName NVARCHAR(500)
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

                -- /Drop Tables
                
                -- Drop Procedures
               
                DECLARE MSGCURSOR CURSOR FOR
                SELECT 
	                schema_name(p.schema_id) as schema_name,
	                p.name as proc_name
                FROM 
	                sys.procedures p
                WHERE 
	                p.name like '{prefix}%'
	
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
                
                -- /Drop Procedures

            ";

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
                        return result.Failed($"A tenant with the name \"{shell.Name}\" already exists! Consider renaming the tenant.");
                    }

                    // Ensure a unique shell location
                    shell = shells.FirstOrDefault(s => s.Location.Equals(context.SiteName.ToSafeFileName(), StringComparison.OrdinalIgnoreCase));
                    if (shell != null)
                    {
                        return result.Failed($"A tenant with the same location \"{shell.Location}\" already exists! Consider renaming the tenant.");
                    }

                    // Ensure a unique connection string & table prefix
                    shell = shells.FirstOrDefault(s =>
                        s.ConnectionString.Equals(context.DatabaseConnectionString, StringComparison.OrdinalIgnoreCase) &&
                        s.TablePrefix.Equals(context.DatabaseTablePrefix, StringComparison.OrdinalIgnoreCase));
                    if (shell != null)
                    {
                        return result.Failed($"A tenant with the same connection string and table prefix already exists! Tenant name: \"{shell.Name}\".");
                    }

                }

                // Install tenant

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

                // Validate tenant

                var shells = _shellSettingsManager.LoadSettings();
                if (shells != null)
                {
                    bool connStringExists = false, 
                        hostExists = false, 
                        prefixExists = false;
                    foreach (var shell in shells)
                    {
                        // Ensure we are checking other shells
                        if (!shell.Name.Equals(context.SiteName, StringComparison.OrdinalIgnoreCase))
                        {

                            if (shell.ConnectionString.Equals(context.DatabaseConnectionString, StringComparison.OrdinalIgnoreCase) &&
                                shell.TablePrefix.Equals(context.DatabaseTablePrefix, StringComparison.OrdinalIgnoreCase))
                            {
                                connStringExists = true;
                            }

                            if (!string.IsNullOrEmpty(shell.RequestedUrlHost))
                            {
                                if (shell.RequestedUrlHost.Equals(context.RequestedUrlHost, StringComparison.OrdinalIgnoreCase)) 
                                {
                                    hostExists = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(shell.RequestedUrlPrefix))
                            {
                                if (shell.RequestedUrlPrefix.Equals(context.RequestedUrlPrefix, StringComparison.OrdinalIgnoreCase)) 
                                {
                                    prefixExists = true;
                                }
                            }

                            if (connStringExists)
                            {
                                return result.Failed($"A tenant with the same connection string and table prefix already exists! Tenant name: \"{shell.Name}\".");
                            }

                            // If we have a host check host and prefix 
                            // Else just check the prefix
                            if (hostExists)
                            {
                                if (hostExists && prefixExists)
                                {
                                    return result.Failed($"A tenant with the same host name and prefix already exists! Tenant name: \"{shell.Name}\".");
                                }
                            }
                            else
                            {
                                if (prefixExists)
                                {
                                    return result.Failed($"A tenant with the same prefix already exists! Tenant name: \"{shell.Name}\".");
                                }
                            }

                        }
                    }                    
                }

                // Update tenant

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
                    
                    var hasErrors = false;
                    void ReportError(string key, string message)
                    {
                        hasErrors = true;
                        context.Errors[key] = message;
                    }

                    // Invoke set-up event handlers
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantSetUpService>>();
                    var setupEventHandlers = scope.ServiceProvider.GetServices<ISetUpEventHandler>();
                    await setupEventHandlers.InvokeAsync(x => x.SetUp(context, ReportError), logger);

                    // Return early if we encountered any errors
                    if (hasErrors)
                    {
                        return result.Failed(context.Errors.Select(e => e.Value).ToArray());                          
                    }

                    // Activate message broker subscriptions for tenant
                    var subscribers = scope.ServiceProvider.GetServices<IBrokerSubscriber>();
                    foreach (var subscriber in subscribers)
                    {
                        subscriber.Subscribe();
                    }

                    // Update tenant email settings
                    var emailSettingsManager = scope.ServiceProvider.GetService<IEmailSettingsManager>();
                    await emailSettingsManager.SaveAsync(context.EmailSettings);

                }

            }

            // Report any errors
            if (context.Errors.Count > 0)
            {
                return result.Failed(context.Errors.Select(e => e.Value).ToArray());           
            }

            // Set shell defaults
            shellSettings.CreatedDate = DateTimeOffset.Now;
            shellSettings.ModifiedDate = DateTimeOffset.Now;
            shellSettings.State = TenantState.Running;

            // Update & recycle shell
            _platoHost
                .UpdateShell(shellSettings)
                .RecycleShell(shellSettings);

            return result.Success(context);

        }

        private Task<ICommandResult<TenantSetUpContext>> UpdateInternalAsync(TenantSetUpContext context)
        {

            var result = new CommandResult<TenantSetUpContext>();

            var shellSettings = BuildShellSettings(context);
            shellSettings.ModifiedDate = DateTimeOffset.Now;

            // Update & recycle shell
            _platoHost
                .UpdateShell(shellSettings)
                .RecycleShell(shellSettings);

            return Task.FromResult(result.Success(context));

        }
      
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

            var errors = new List<CommandError>();

            // ----------------------
            // 1. Attempt to delete App_Data/{SiteName} folder
            // ----------------------

            var deleted = true;
            try
            {
                deleted = _shellSettingsManager.DeleteSettings(shellSettings);
            }
            catch (Exception e)
            {
                errors.Add(new CommandError(e.Message));
            }

            // Report any errors
            if (errors.Count > 0)
            {
                return result.Failed(errors.ToArray());
            }

            // Ensure we could delete the directory
            if (deleted == false)
            {
                return result.Failed($"Cannot delete tenant folder with the name \"{shellSettings.Location}\"!");
            }

            // ----------------------
            // 2. Attempt to drop all tables and stored procedures with our table prefix
            // ----------------------

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
                            await dbHelper.ExecuteScalarAsync<int>(UninstallSql, new Dictionary<string, string>()
                            {
                                ["{prefix}"] = shellSettings.TablePrefix
                            });
                        }
                        catch (Exception e)
                        {
                            errors.Add(new CommandError(e.Message));
                        }

                    }
                }
            }

            // ----------------------
            // 3. Dispose the tenant
            // ----------------------

            // Ensure no errors occurred
            if (errors.Count == 0)
            {
                _platoHost.DisposeShell(shellSettings);
            }

            return errors.Count > 0
                ? result.Failed(errors.ToArray())
                : result.Success();

        }

        private ShellSettings BuildShellSettings(TenantSetUpContext context)
        {

            var shellSettings = new ShellSettings()
            {
                Name = context.SiteName,
                Location = !string.IsNullOrEmpty(context.Location) 
                    ? context.Location 
                    : context.SiteName.ToSafeFileName(),
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
