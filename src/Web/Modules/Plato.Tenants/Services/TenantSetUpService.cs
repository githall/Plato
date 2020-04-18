using System;
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

    public interface ITenantSetUpService
    {

        Task<ICommandResult<ISetUpContext>> InstallAsync(TenantSetUpContext context);

        Task<ICommandResult<ISetUpContext>> UninstallAsync(TenantSetUpContext context);

    }

    public class TenantSetUpService : ITenantSetUpService
    {

        private const string TablePrefixSeparator = "_";

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
   
        private readonly IPlatoHost _platoHost;

        public TenantSetUpService(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,  
            IPlatoHost platoHost)
        {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;       
            _platoHost = platoHost;
        }

        public async Task<ICommandResult<ISetUpContext>> InstallAsync(TenantSetUpContext context)
        {

            var result = new CommandResult<ISetUpContext>();

            // Validate tenant

            var shells = _shellSettingsManager.LoadSettings();

            if (shells != null)
            {

                // Ensure a unique shell name
                var shell = shells.First(s => s.Name.Equals(context.SiteName, StringComparison.OrdinalIgnoreCase));
                if (shell != null)
                {
                    return result.Failed($"A tenant with the name \"{shell.Name}\" already exists!");
                  
                }

                // Ensure a unique connection string & table prefix
                shell = shells.First(s =>
                    s.ConnectionString.Equals(context.DatabaseConnectionString, StringComparison.OrdinalIgnoreCase) &&
                    s.TablePrefix.Equals(context.DatabaseTablePrefix, StringComparison.OrdinalIgnoreCase));
                if (shell != null)
                {
                    return result.Failed($"A tenant with the same connection string and table prefix already exists!");           
                }

            }
        
            // Configure tenant

            try
            {
                return await InstallInternalAsync(context);
            }
            catch (Exception ex)
            {
                return result.Failed(ex.Message);
            }

        }

        public Task<ICommandResult<ISetUpContext>> UninstallAsync(TenantSetUpContext context)
        {
            var result = new CommandResult<ISetUpContext>();

            return Task.FromResult(result.Failed());

        }

        // --------------------------


        async Task<ICommandResult<ISetUpContext>> InstallInternalAsync(ISetUpContext context)
        {

            var result = new CommandResult<ISetUpContext>();

            // Set state to "Initializing" so that subsequent HTTP requests are responded to with "Service Unavailable" while setting up.
            //_shellSettings.State = TenantState.Initializing;

            var executionId = Guid.NewGuid().ToString("n");

            var shellSettings = new ShellSettings()
            {
                Name = context.SiteName,
                Location = context.SiteName.ToSafeFileName(),
                RequestedUrlHost = context.RequestedUrlHost,
                RequestedUrlPrefix = context.RequestedUrlPrefix,
                State = TenantState.Initializing
            };

            if (string.IsNullOrEmpty(shellSettings.DatabaseProvider))
            {
                var tablePrefix = context.DatabaseTablePrefix;
                if (!tablePrefix.EndsWith(TablePrefixSeparator))
                    tablePrefix += TablePrefixSeparator;
                shellSettings.DatabaseProvider = context.DatabaseProvider;
                shellSettings.ConnectionString = context.DatabaseConnectionString;
                shellSettings.TablePrefix = tablePrefix;
            }

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

                        // Invoke modules to react to the setup event
                     
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantSetUpService>>();

                        var setupEventHandlers = scope.ServiceProvider.GetServices<ISetUpEventHandler>();
                        await setupEventHandlers.InvokeAsync(x => x.SetUp(context, ReportError), logger);

                        if (hasErrors)
                        {
                            return result.Failed(context.Errors.Select(e => e.Value).ToArray());                          
                        }

                        var shellSettingsManager = scope.ServiceProvider.GetService<IShellSettingsManager>();
                        shellSettings.State = TenantState.Running;
                        shellSettingsManager.SaveSettings(shellSettings);

                    }

                }

            }

            if (context.Errors.Count > 0)
            {
                return result.Failed(context.Errors.Select(e => e.Value).ToArray());           
            }
            
            return result.Success(context);

        }

    }

}
