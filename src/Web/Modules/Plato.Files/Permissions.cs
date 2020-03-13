using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission BrowseFiles =
            new Permission("BrowseFiles", "Browse files");

        public static readonly Permission AddFiles =
            new Permission("AddFiles", "Add files");

        public static readonly Permission EditFiles =
            new Permission("EditFiles", "Edit files");

        public static readonly Permission ShareFiles =
            new Permission("ShareFiles", "Share files");

        public static readonly Permission DeleteFiles =
            new Permission("DeleteFiles", "Delete files");

        public static readonly Permission ManageFileSettings =
            new Permission("ManageFileSettings", "Manage file settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                BrowseFiles,
                AddFiles,
                EditFiles,
                ShareFiles,
                DeleteFiles,
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
                        BrowseFiles,
                        AddFiles,
                        EditFiles,
                        ShareFiles,
                        DeleteFiles,
                        ManageFileSettings
                    }
                }
            };

        }

    }

}
