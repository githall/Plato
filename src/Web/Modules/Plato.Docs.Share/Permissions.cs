using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Docs.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareDocs =
            new Permission("ShareDocs", "Share docs");

        public static readonly Permission ShareComments =
            new Permission("ShareDocComments", "Share doc comments");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareDocs,
                ShareComments
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
                        ShareDocs,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareDocs,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ShareDocs,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareDocs,
                        ShareComments
                    }
                }
            };

        }

    }

}
