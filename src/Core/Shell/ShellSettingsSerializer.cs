using System;
using Microsoft.Extensions.Configuration;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell
{

    public static class ShellSettingsSerializer
    {

        public static ShellSettings ParseSettings(IConfigurationRoot configuration)
        {

            var settings = new ShellSettings
            {
                Name = configuration["Name"],         
                ConnectionString = configuration["ConnectionString"],
                TablePrefix = configuration["TablePrefix"],
                DatabaseProvider = configuration["DatabaseProvider"],
                Theme = configuration["Theme"],
                RequestedUrlHost = configuration["RequestedUrlHost"],
                RequestedUrlPrefix = configuration["RequestedUrlPrefix"],
                State = Enum.TryParse(configuration["State"], true, out TenantState state)
                    ? state
                    : TenantState.Uninitialized,
                OwnerId = configuration["OwnerId"],
            };

            if (DateTime.TryParse(configuration["CreatedDate"], out var createdDate)) 
            {
                settings.CreatedDate = createdDate;
            }

            if (DateTime.TryParse(configuration["ModifiedDate"], out var modifiedDate))
            {
                settings.ModifiedDate = modifiedDate;
            }

            return settings;

        }

    }

}
