using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarArticles =
            new Permission("StarArticles", "Star articles");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarArticles
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
                        StarArticles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarArticles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarArticles
                    }
                }
            };

        }

    }

}
