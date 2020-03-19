using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadIdeaFiles =
            new Permission("DownloadIdeaFiles", "Download idea files");

        public static readonly Permission PostIdeaFiles =
            new Permission("PostIdeaFiles", "Post idea files");

        public static readonly Permission DeleteOwnIdeaFiles =
            new Permission("DeleteOwnIdeaFiles", "Delete own idea files");

        public static readonly Permission DeleteAnyIdeaFile =
            new Permission("DeleteAnyIdeaFile", "Delete any idea files");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadIdeaFiles,
                PostIdeaFiles,
                DeleteOwnIdeaFiles,
                DeleteAnyIdeaFile          
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
                        DownloadIdeaFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadIdeaFiles,
                        PostIdeaFiles,
                        DeleteOwnIdeaFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadIdeaFiles,
                        PostIdeaFiles,
                        DeleteOwnIdeaFiles,
                        DeleteAnyIdeaFile
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadIdeaFiles,
                        PostIdeaFiles,
                        DeleteOwnIdeaFiles,
                        DeleteAnyIdeaFile
                    }
                }
            };

        }

    }

}
