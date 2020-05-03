using System;
using System.Threading;
using System.Collections.Generic;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using Microsoft.Extensions.Logging;

namespace PlatoCore.Shell
{

    public class RunningShellTable : IRunningShellTable
    {

        private readonly Dictionary<string, IShellSettings> _shellsByHostAndPrefix =
            new Dictionary<string, IShellSettings>(StringComparer.OrdinalIgnoreCase);
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private IShellSettings _default;

        private readonly ILogger<RunningShellTable> _logger;


        public RunningShellTable(ILogger<RunningShellTable> logger)
        {
            _logger = logger;
        }


        public void Add(IShellSettings settings)
        {

            _lock.EnterWriteLock();
            try
            {

                // Set default shell
                if (ShellHelper.DefaultShellName.Equals(settings.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"The default tenant is located at \"\\App_Data\\Sites\\{settings.Location}\".");
                    }
                    _default = settings;
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError($"No default tenant could be found. Ensure a tenant with the name \"{ShellHelper.DefaultShellName}\" exists within the \"\\App_Data\\Sites\\\" folder.");
                    }
                }

                var hostAndPrefix = GetHostAndPrefix(settings);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Adding URL of \"{hostAndPrefix}\" for tenant \"{settings.Name}\" to running shell table.");
                }

                _shellsByHostAndPrefix[hostAndPrefix] = settings;


            }
            finally
            {
                _lock.ExitWriteLock();
            }

        }

        public void Remove(IShellSettings settings)
        {

            _lock.EnterWriteLock();
            try
            {

                var hostAndPrefix = GetHostAndPrefix(settings);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Removing URL \"{hostAndPrefix}\" for tenant \"{settings.Name}\" from running shell table.");
                }

                _shellsByHostAndPrefix.Remove(hostAndPrefix);
                if (_default == settings)
                {
                    _default = null;
                }

            }
            finally
            {
                _lock.ExitWriteLock();
            }

        }

        public IShellSettings Match(string host, string appRelativePath)
        {

            _lock.EnterReadLock();
            try
            {

                var hostAndPrefix = GetHostAndPrefix(host, appRelativePath);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Attempting to resolve tenant with URL \"{hostAndPrefix}\" within running shell table.");
                }

                if (!_shellsByHostAndPrefix.TryGetValue(hostAndPrefix, out var result))
                {

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Failed to resolve tenant. No tenant matched the URL \"{hostAndPrefix}\" within running shell table.");
                    }

                    var noHostAndPrefix = GetHostAndPrefix("", appRelativePath);

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Falling back to use the default tenant with the URL \"{noHostAndPrefix}\" within the running shell table.");
                    }

                    if (!_shellsByHostAndPrefix.TryGetValue(noHostAndPrefix, out result))
                    {

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation($"No tenant with the URL \"{noHostAndPrefix}\" exists within running shell table. Falling back to default tenant.");
                        }

                        if (_default == null)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError($"No default tenant could be found. Ensure a tenant with the name \"{ShellHelper.DefaultShellName}\" exists within the \"\\App_Data\\Sites\\\" folder.");
                            }
                        }

                        result = _default;

                    }

                }

                return result;

            }
            finally
            {
                _lock.ExitReadLock();
            }

        }

        public IDictionary<string, IShellSettings> ShellsByHostAndPrefix
            => _shellsByHostAndPrefix;

        private string GetHostAndPrefix(string host, string appRelativePath)
        {

            // removing the port from the host
            var hostLength = host.IndexOf(':');
            if (hostLength != -1)
            {
                host = host.Substring(0, hostLength);
            }

            // appRelativePath starts with /
            int firstSegmentIndex = appRelativePath.IndexOf('/', 1);
            if (firstSegmentIndex > -1)
            {
                return host + appRelativePath.Substring(0, firstSegmentIndex);
            }

            return host + appRelativePath;

        }

        private string GetHostAndPrefix(IShellSettings shellSettings)
        {
            return shellSettings.RequestedUrlHost + "/" + shellSettings.RequestedUrlPrefix;
        }

    }

}
