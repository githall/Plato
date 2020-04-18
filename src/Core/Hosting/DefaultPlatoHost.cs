﻿using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Tasks.Abstractions;

namespace PlatoCore.Hosting
{

    public class DefaultPlatoHost : IPlatoHost
    {

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IBackgroundTaskManager _taskManager;
        private readonly IBroker _broker;
        private readonly ILogger _logger;
        
        private ConcurrentDictionary<string, ShellContext> _shellContexts;
        private static readonly object SyncLock = new object();

        public DefaultPlatoHost(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable,             
            IBackgroundTaskManager taskManager,
            ILogger<DefaultPlatoHost> logger,
            IBroker broker)
        {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;
            _runningShellTable = runningShellTable;        
            _taskManager = taskManager;
            _logger = logger;
            _broker = broker;
        }

        // Implementation

        public void Initialize()
        {
            BuildCurrent();
        }

        public ShellContext GetOrCreateShellContext(IShellSettings settings)
        {
            if (_shellContexts == null)
            {
                _shellContexts = new ConcurrentDictionary<string, ShellContext>();
            }
                
            return _shellContexts.GetOrAdd(settings.Name, tenant =>
            {
                var shellContext = CreateShellContext(settings);
                ActivateShell(shellContext);
                return shellContext;
            });

        }

        public void UpdateShellSettings(IShellSettings settings)
        {
            _shellSettingsManager.SaveSettings(settings);
            RecycleShellContext(settings);
        }

        public ShellContext CreateShellContext(IShellSettings settings)
        {
            if (settings.State == TenantState.Uninitialized)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Creating shell context for tenant {0} setup", settings.Name);
                }
                return _shellContextFactory.CreateSetupContext(settings);
            }

            _logger.LogDebug("Creating shell context for tenant {0}", settings.Name);
            return _shellContextFactory.CreateShellContext(settings);
        }

        public void RecycleShellContext(IShellSettings settings)
        {

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Recycling shell context for tenant {0}", settings.Name);
            }

            // Dispose
            DisposeShellContext(settings);

            // Recreate
            GetOrCreateShellContext(settings);

        }

        public void DisposeShellContext(IShellSettings settings)
        {

            // Dispose
            _runningShellTable.Remove(settings);
            if (_shellContexts.TryRemove(settings.Name, out var context))
            {
                if (_shellContexts.Count == 0)
                    _shellContexts = null;
                context.Dispose();
            }

            // Dispose all shell message broker subscriptions 
            // These will be activated again via BuildTenantPipeline within
            // Plato.Internal.Hosting.Web.Routing.PlatoRouterMiddleware
            _broker?.Dispose();

            // Stop all shell background tasks
            // These will be activated again via BuildTenantPipeline within
            // Plato.Internal.Hosting.Web.Routing.PlatoRouterMiddleware
            _taskManager.StopTasks();

        }

        // Private methods

        private IDictionary<string, ShellContext> BuildCurrent()
        {

            if (_shellContexts == null)
            {
                lock (SyncLock)
                {
                    if (_shellContexts == null)
                    {
                        _shellContexts = new ConcurrentDictionary<string, ShellContext>();
                        CreateAndActivateShells();
                    }
                }
            }

            return _shellContexts;
        }

        private void CreateAndActivateShells()
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Start creation of shells");
            }                
            
            // Is there any tenant right now?
            var allSettings = _shellSettingsManager
                .LoadSettings()
                .Where(CanCreateShell).ToArray();
            
            // Load all tenants, and activate their shell.
            if (allSettings.Any())
            {
                //Parallel.ForEach(allSettings, settings =>
                //{
                foreach (var settings in allSettings)
                {
                    GetOrCreateShellContext(settings);
                }
                //});
            }
            else
            {
                // No settings, run the Setup.
                var setupContext = CreateSetupContext();
                ActivateShell(setupContext);
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Done creating shells");
            }
        }

        private void ActivateShell(ShellContext context)
        {

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Activating context for tenant {0}", context.Settings.Name);
            }                

            if (_shellContexts.TryAdd(context.Settings.Name, context))
            {
                _runningShellTable.Add(context.Settings);
            }

        }

        private ShellContext CreateSetupContext()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Creating shell context for root setup.");
            return _shellContextFactory.CreateSetupContext(ShellHelper.BuildDefaultUninitializedShell);
        }

        private bool CanCreateShell(IShellSettings shellSettings)
        {
            return
                shellSettings.State == TenantState.Running ||
                shellSettings.State == TenantState.Uninitialized ||
                shellSettings.State == TenantState.Initializing ||
                shellSettings.State == TenantState.Disabled;
        }

    }

}
