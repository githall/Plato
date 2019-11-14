using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Google
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditGoogleSettings =
            new Permission("EditGoogleSettings", "Manage google settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditGoogleSettings
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
                        EditGoogleSettings
                    }
                }
            };

        }

    }

}
