﻿using Microsoft.Extensions.Configuration;
using Plato.Shell.Models;
using System;

namespace Plato.Shell
{
    public static class ShellSettingsSerializer
    {
        public static ShellSettings ParseSettings(IConfigurationRoot configuration)
        {
            var shellSettings = new ShellSettings();

            shellSettings.Name = configuration["Name"];            
            shellSettings.HostName = configuration["HostName"];
            shellSettings.SubDomain = configuration["SubDomain"];
            shellSettings.ConnectionString = configuration["ConnectionString"];
            shellSettings.TablePrefix = configuration["TablePrefix"];
            shellSettings.DatabaseProvider = configuration["DatabaseProvider"];
            shellSettings.Theme = configuration["Theme"];

            TenantState state;
            shellSettings.State = Enum.TryParse(configuration["State"], true, out state) ? state : TenantState.Uninitialized;

            return shellSettings;
        }
    }
}
