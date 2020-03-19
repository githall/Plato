using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadDiscussFiles =
            new Permission("DownloadDiscussFiles", "Download topic files");

        public static readonly Permission PostDiscussFiles =
            new Permission("PostDiscussFiles", "Post topic files");

        public static readonly Permission DeleteOwnDiscussFiles =
            new Permission("DeleteOwnDiscussFiles", "Delete own topic files");

        public static readonly Permission DeleteAnyDiscussFile =
            new Permission("DeleteAnyDiscussFile", "Delete any topic file");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadDiscussFiles,
                PostDiscussFiles,
                DeleteOwnDiscussFiles,
                DeleteAnyDiscussFile          
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
                        DownloadDiscussFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadDiscussFiles,
                        PostDiscussFiles,
                        DeleteOwnDiscussFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadDiscussFiles,
                        PostDiscussFiles,
                        DeleteOwnDiscussFiles,
                        DeleteAnyDiscussFile
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadDiscussFiles,
                        PostDiscussFiles,
                        DeleteOwnDiscussFiles,
                        DeleteAnyDiscussFile
                    }
                }
            };

        }

    }

}
