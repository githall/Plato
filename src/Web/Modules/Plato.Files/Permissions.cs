using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageFiles =
            new Permission("ManageFiles", "Browse files");

        public static readonly Permission ManageFileSettings =
            new Permission("ManageFileSettings", "Edit attachment settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageFiles,
                ManageFileSettings
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
                        ManageFiles,
                        ManageFileSettings
                    }
                }
            };

        }

    }

}
