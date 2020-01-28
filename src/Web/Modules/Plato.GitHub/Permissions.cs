using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.GitHub
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditGitHubSettings =
            new Permission("EditGitHubSettings", "Manage google settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditGitHubSettings
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
                        EditGitHubSettings
                    }
                }
            };

        }

    }

}
