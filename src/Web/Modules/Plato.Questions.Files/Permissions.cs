using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.Files
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadQuestionFiles =
            new Permission("DownloadQuestionFiles", "Download question files");

        public static readonly Permission PostQuestionFiles =
            new Permission("PostQuestionFiles", "Post question files");

        public static readonly Permission DeleteOwnQuestionFiles =
            new Permission("DeleteOwnQuestionFiles", "Delete own question files");

        public static readonly Permission DeleteAnyQuestionFiles =
            new Permission("DeleteAnyQuestionFiles", "Delete any question files");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadQuestionFiles,
                PostQuestionFiles,
                DeleteOwnQuestionFiles,
                DeleteAnyQuestionFiles
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
                        DownloadQuestionFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadQuestionFiles,
                        PostQuestionFiles,
                        DeleteOwnQuestionFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadQuestionFiles,
                        PostQuestionFiles,
                        DeleteOwnQuestionFiles,
                        DeleteAnyQuestionFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadQuestionFiles,
                        PostQuestionFiles,
                        DeleteOwnQuestionFiles,
                        DeleteAnyQuestionFiles
                    }
                }
            };

        }

    }

}
