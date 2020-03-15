using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Docs.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadDocFiles =
            new Permission("DownloadDocFiles", "Download doc files");

        public static readonly Permission PostDocFiles =
            new Permission("PostDocFiles", "Post doc files");

        public static readonly Permission DeleteOwnDocFiles =
            new Permission("DeleteOwnDocFiles", "Delete own doc files");

        public static readonly Permission DeleteAnyDocFiles =
            new Permission("DeleteAnyDocFiles", "Delete any doc files");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadDocFiles,
                PostDocFiles,
                DeleteOwnDocFiles,
                DeleteAnyDocFiles          
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        DownloadDocFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadDocFiles,
                        PostDocFiles,
                        DeleteOwnDocFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadDocFiles,
                        PostDocFiles,
                        DeleteOwnDocFiles,
                        DeleteAnyDocFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadDocFiles,
                        PostDocFiles,
                        DeleteOwnDocFiles,
                        DeleteAnyDocFiles
                    }
                }
            };

        }

    }

}
