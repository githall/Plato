using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Attachments
{

    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission DownloadArticleAttachments =
            new Permission("DownloadArticleAttachments", "Download article attachments");

        public static readonly Permission PostArticleAttachments =
            new Permission("PostArticleAttachments", "Post article attachments");

        public static readonly Permission DeleteOwnArticleAttachments =
            new Permission("DeleteOwnArticleAttachments", "Delete own article attachments");

        public static readonly Permission DeleteAnyArticleAttachments =
            new Permission("DeleteAnyArticleAttachments", "Delete any article attachments");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DownloadArticleAttachments,
                PostArticleAttachments,
                DeleteOwnArticleAttachments,
                DeleteAnyArticleAttachments          
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
                        DownloadArticleAttachments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        DownloadArticleAttachments,
                        PostArticleAttachments,
                        DeleteOwnArticleAttachments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        DownloadArticleAttachments,
                        PostArticleAttachments,
                        DeleteOwnArticleAttachments,
                        DeleteAnyArticleAttachments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        DownloadArticleAttachments,
                        PostArticleAttachments,
                        DeleteOwnArticleAttachments,
                        DeleteAnyArticleAttachments
                    }
                }
            };

        }

    }

}
