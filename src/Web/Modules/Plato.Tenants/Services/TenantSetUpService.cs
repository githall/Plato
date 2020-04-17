using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Tenants.Models;
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
        Task<string> SetUpAsync(TenantSetUpContext context);
    }

    public class TenantSetUpService : ITenantSetUpService
    {

        private const string TablePrefixSeparator = "_";

        private readonly IShellContextFactory _shellContextFactory;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public TenantSetUpService(
            IShellContextFactory shellContextFactory,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _shellContextFactory = shellContextFactory;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
        }

        public async Task<string> SetUpAsync(TenantSetUpContext context)
        {
            //var initialState = _shellSettings.State;
            try
            {
                return await SetUpInternalAsync(context);
            }
            catch (Exception ex)
            {
                context.Errors.Add(ex.Message, ex.Message);
                //_shellSettings.State = initialState;
                throw;
            }
        }

        // --------------------------


        async Task<string> SetUpInternalAsync(ISetUpContext context)
        {

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
                            context.Errors[key] = message;
                        }

                        // Invoke modules to react to the setup event
                     
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantSetUpService>>();

                        var setupEventHandlers = scope.ServiceProvider.GetServices<ISetUpEventHandler>();
                        await setupEventHandlers.InvokeAsync(x => x.SetUp(context, ReportError), logger);

                        if (hasErrors)
                        {
                            return executionId;
                        }

                        var shellSettingsManager = scope.ServiceProvider.GetService<IShellSettingsManager>();
                        shellSettings.State = TenantState.Running;
                        shellSettingsManager.SaveSettings(shellSettings);

                    }

                }

            }

            if (context.Errors.Count > 0)
            {
                return executionId;
            }
            
            return executionId;

        }

    }

}
