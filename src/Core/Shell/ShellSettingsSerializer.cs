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
                RequestedUrlHost = configuration["RequestedUrlHost"],
                RequestedUrlPrefix = configuration["RequestedUrlPrefix"],        
                OwnerId = configuration["OwnerId"],
                Theme = configuration["Theme"]
            };

            if (Enum.TryParse(configuration["State"], true, out TenantState state))
            {
                settings.State = state;
            }

            if (DateTimeOffset.TryParse(configuration["CreatedDate"], out var createdDate)) 
            {
                settings.CreatedDate = createdDate;
            }

            if (DateTimeOffset.TryParse(configuration["ModifiedDate"], out var modifiedDate))
            {
                settings.ModifiedDate = modifiedDate;
            }

            return settings;

        }

    }

}
