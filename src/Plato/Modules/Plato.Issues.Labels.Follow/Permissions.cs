using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Labels.Follow
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowDiscussLabels =
            new Permission("FollowIssueLabels", "Can follow labels");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowDiscussLabels
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
                        FollowDiscussLabels
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowDiscussLabels
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowDiscussLabels
                    }
                }
            };

        }

    }

}
