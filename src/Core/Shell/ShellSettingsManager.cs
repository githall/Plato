using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using PlatoCore.Yaml.Extensions;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Yaml;

namespace PlatoCore.Shell
{

    public class ShellSettingsManager : IShellSettingsManager
    {

        private const string SettingsFileNameFormat = "Settings.{0}";

        private readonly IOptions<ShellOptions> _optionsAccessor;
        private readonly IAppDataFolder _appDataFolder;        
        private readonly ILogger _logger;

        public ShellSettingsManager(
            IAppDataFolder appDataFolder,
            IOptions<ShellOptions> optionsAccessor,
            ILogger<ShellSettingsManager> logger)
        {
            _optionsAccessor = optionsAccessor;
            _appDataFolder = appDataFolder;
            _logger = logger;
        }

        // Implementation

        public IEnumerable<ShellSettings> LoadSettings()
        {

            var shellSettings = new List<ShellSettings>();
            foreach (var directory in _appDataFolder.ListDirectories(_optionsAccessor.Value.Location))
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("ShellSettings found in '{0}', attempting to load.", directory.Name);
                }
                var configurationContainer =
                    new ConfigurationBuilder()
                        .SetBasePath(_appDataFolder.RootPath)
                        .AddJsonFile(_appDataFolder.Combine(directory.FullName, string.Format(SettingsFileNameFormat, "json")),
                            true)
                        .AddXmlFile(_appDataFolder.Combine(directory.FullName, string.Format(SettingsFileNameFormat, "xml")),
                            true)
                        .AddYamlFile(_appDataFolder.Combine(directory.FullName, string.Format(SettingsFileNameFormat, "txt")),
                            false);

                var config = configurationContainer.Build();
                var shellSetting = ShellSettingsSerializer.ParseSettings(config);
                shellSetting.Location = directory.Name;
                shellSettings.Add(shellSetting);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Loaded ShellSettings for tenant '{0}'", shellSetting.Name);
                }
            }

            return shellSettings;

        }

        public void SaveSettings(IShellSettings shellSettings)
        {

            if (shellSettings == null)
            {
                throw new ArgumentNullException(nameof(shellSettings));
            }

            if (string.IsNullOrEmpty(shellSettings.Name))
            {
                throw new ArgumentNullException(nameof(shellSettings.Name));
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saving shell settings for tenant '{0}'", shellSettings.Name);
            }

            var fileName = string.Format(SettingsFileNameFormat, "txt");
            var tenantPath = _appDataFolder.MapPath(
                _appDataFolder.Combine(
                    _optionsAccessor.Value.Location,
                    shellSettings.Location,
                    fileName));

            var configurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource
            {
                Path = tenantPath,
                Optional = false
            });

            foreach (var key in shellSettings.Keys)
            {
                if (!string.IsNullOrEmpty(shellSettings[key]))
                {
                    configurationProvider.Set(key, shellSettings[key]);
                }
            }

            configurationProvider.Commit();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saved shell settings for tenant '{0}'", shellSettings.Name);
            }

        }

    }

}
