using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Attachments
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadArticleFiles =
            new Permission("DownloadArticleFiles", "Download article files");

        public static readonly Permission PostArticleFiles =
            new Permission("PostArticleFiles", "Post article files");

        public static readonly Permission DeleteOwnArticleFiles =
            new Permission("DeleteOwnArticleFiles", "Delete own article files");

        public static readonly Permission DeleteAnyArticleFiles =
            new Permission("DeleteAnyArticleFiles", "Delete any article files");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadArticleFiles,
                PostArticleFiles,
                DeleteOwnArticleFiles,
                DeleteAnyArticleFiles          
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
                        DownloadArticleFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadArticleFiles,
                        PostArticleFiles,
                        DeleteOwnArticleFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadArticleFiles,
                        PostArticleFiles,
                        DeleteOwnArticleFiles,
                        DeleteAnyArticleFiles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadArticleFiles,
                        PostArticleFiles,
                        DeleteOwnArticleFiles,
                        DeleteAnyArticleFiles
                    }
                }
            };

        }

    }

}
