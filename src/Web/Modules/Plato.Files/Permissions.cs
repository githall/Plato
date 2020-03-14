using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission BrowseFiles =
            new Permission("BrowseFiles", "Browse files");

        public static readonly Permission DownloadFiles =
            new Permission("DownloadFiles", "Download files");

        public static readonly Permission OpenFiles =
            new Permission("OpenFiles", "Open files");

        public static readonly Permission AddFiles =
            new Permission("AddFiles", "Add files");

        public static readonly Permission EditOwnFiles =
            new Permission("EditOwnFiles", "Edit own files");

        public static readonly Permission EditAnyFile =
            new Permission("EditAnyFile", "Edit any file");

        public static readonly Permission DeleteOwnFiles =
            new Permission("DeleteOwnFiles", "Delete own files");

        public static readonly Permission DeleteAnyFile =
            new Permission("DeleteAnyFile", "Delete any file");

        public static readonly Permission ManageFileSettings =
            new Permission("ManageFileSettings", "Manage file settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                BrowseFiles,
                DownloadFiles,
                OpenFiles,
                AddFiles,            
                EditOwnFiles,
                EditAnyFile,
                DeleteOwnFiles,
                DeleteAnyFile,
                ManageFileSettings
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        BrowseFiles,
                        DownloadFiles,
                        OpenFiles,
                        AddFiles,                       
                        EditOwnFiles,
                        DeleteOwnFiles                        
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        BrowseFiles,
                        DownloadFiles,
                        OpenFiles,
                        AddFiles,                        
                        EditOwnFiles,
                        EditAnyFile,
                        DeleteOwnFiles,
                        DeleteAnyFile,
                        ManageFileSettings
                    }
                }
            };

        }

    }

}
