using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageAttachments =
            new Permission("ManageAttachments", "Browse files");

        public static readonly Permission ManageAttachmentSettings =
            new Permission("ManageAttachmentSettings", "Edit attachment settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageAttachments,
                ManageAttachmentSettings
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
                        ManageAttachments,
                        ManageAttachmentSettings
                    }
                }
            };

        }

    }

}
