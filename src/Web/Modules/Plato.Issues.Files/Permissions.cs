using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Issues.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadIssueFiles =
            new Permission("DownloadIssueFiles", "Download issue files");

        public static readonly Permission PostIssueFiles =
            new Permission("PostIssueFiles", "Post issue files");

        public static readonly Permission DeleteOwnIssueFiles =
            new Permission("DeleteOwnIssueFiles", "Delete own issue files");

        public static readonly Permission DeleteAnyIssueFile =
            new Permission("DeleteAnyIssueFile", "Delete any issue files");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadIssueFiles,
                PostIssueFiles,
                DeleteOwnIssueFiles,
                DeleteAnyIssueFile          
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
                        DownloadIssueFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadIssueFiles,
                        PostIssueFiles,
                        DeleteOwnIssueFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadIssueFiles,
                        PostIssueFiles,
                        DeleteOwnIssueFiles,
                        DeleteAnyIssueFile
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadIssueFiles,
                        PostIssueFiles,
                        DeleteOwnIssueFiles,
                        DeleteAnyIssueFile
                    }
                }
            };

        }

    }

}
