using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Issues.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarIssues =
            new Permission("StarIssues", "Star issues");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarIssues
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
                        StarIssues
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarIssues
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarIssues
                    }
                }
            };

        }

    }

}
