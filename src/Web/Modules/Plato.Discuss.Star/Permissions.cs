using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarTopics =
            new Permission("StarTopics", "Star topics");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarTopics
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
                        StarTopics
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarTopics
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarTopics
                    }
                }
            };

        }

    }

}
