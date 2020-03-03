using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Attachments
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageAttachmentSettings =
            new Permission("ManageAttachmentSettings", "Manage attachment settings");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
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
                        ManageAttachmentSettings
                    }
                }
            };

        }

    }

}
