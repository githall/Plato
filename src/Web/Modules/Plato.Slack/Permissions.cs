using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Slack
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditSlackSettings =
            new Permission("EditSlackSettings", "Manage Slack settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditSlackSettings
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        EditSlackSettings
                    }
                }
            };

        }

    }

}
