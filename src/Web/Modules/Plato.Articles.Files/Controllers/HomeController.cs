using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PlatoCore.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Files.Stores;
using Plato.Entities.Models;
using Plato.Files.Models;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.Models;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using PlatoCore.Layout.ModelBinding;
using Plato.Entities.Files.ViewModels;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Services;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Articles.Files.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {

        public const string ModuleId = "Plato.Articles.Files";

        private readonly IFileViewIncrementer<File> _fileViewIncrementer;
        private readonly IEntityFileStore<EntityFile> _entityFileStore;        
        private readonly IAuthorizationService _authorizationService;        
        private readonly IEntityStore<Article> _entityStore;
        private readonly IFileStore<File> _fileStore;

        public HomeController(
            IFileViewIncrementer<File> fileViewIncrementer,
            IEntityFileStore<EntityFile> entityFileStore,            
            IAuthorizationService authorizationService,
            IEntityStore<Article> entityStore,
            IFileStore<File> fileStore)
        {
            _authorizationService = authorizationService;
            _fileViewIncrementer = fileViewIncrementer;
            _entityFileStore = entityFileStore;
            _entityStore = entityStore;
            _fileStore = fileStore;
        }

        // ----------
        // Download
        // ----------

        [HttpGet, AllowAnonymous]
        public async Task Download(int id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DownloadArticleFiles))
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.Unauthorized);
                return;
            }

            // Get file
            var file = await _fileStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (file == null)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Do we have permission to view at least one of the
            // entities the file is associated with
            if (!await AuthorizeAsync(file))
            {             
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.Unauthorized);
                return;
            }

            if (file.ContentLength <= 0)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Increment file view count
            await _fileViewIncrementer
                .Contextulize(HttpContext)
                .IncrementAsync(file);

            // Expire entity files cache to ensure view count is reflected correctly
            _entityFileStore.CancelTokens(null);

            // Serve file
            Response.Clear();
            Response.ContentType = file.ContentType;
            Response.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + file.Name + "\"");
            Response.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)file.ContentLength));
            await Response.Body.WriteAsync(file.ContentBlob, 0, (int)file.ContentLength);     

        }

        // -----------
        // Edit
        // -----------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Edit(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostArticleFiles;
            opts.DeleteOwnPermission = Permissions.DeleteOwnArticleFiles;
            opts.DeleteAnyPermission = Permissions.DeleteAnyArticleFiles;

            opts.DeleteRoute = new RouteValueDictionary()
            {
                ["area"] = ModuleId,
                ["controller"] = "Api",
                ["action"] = "Delete"
            };

            // Return view
            return Task.FromResult((IActionResult)View(opts));


        }

        // -----------
        // Preview
        // -----------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Preview(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostArticleFiles;
            opts.DeleteOwnPermission = Permissions.DeleteOwnArticleFiles;
            opts.DeleteAnyPermission = Permissions.DeleteAnyArticleFiles;

            opts.DeleteRoute = new RouteValueDictionary()
            {
                ["area"] = ModuleId,
                ["controller"] = "Api",
                ["action"] = "Delete"
            };

            // Return view
            return Task.FromResult((IActionResult) View(opts));

        }

        // ------------------------------

        async Task<bool> AuthorizeAsync(File attachment)
        {

            // Get all entities associated with the attachment
            var relationships = await _entityFileStore
                .QueryAsync()
                .Select<EntityFileQueryParams>(q =>
                {
                    q.FileId.Equals(attachment.Id);
                })
                .ToList();

            // Deny access by default
            var authorized = false;

            if (relationships?.Data != null)
            {

                // Get all related entities
                var entities = await _entityStore
                    .QueryAsync()
                    .Select<EntityQueryParams>(q =>
                    {
                        q.Id.IsIn(relationships.Data.Select(r => r.EntityId).ToArray());
                    })
                    .ToList();

                if (entities?.Data != null)
                {
                    // If any of the authorization checks pass allow access         
                    foreach (var entity in entities.Data)
                    {
                        var authorizeResult = await AuthorizeAsync(entity);
                        if (authorizeResult.Succeeded)
                        {
                            authorized = true;
                        }
                    }
                }
            }

            return authorized;

        }

        async Task<ICommandResultBase> AuthorizeAsync(IEntity entity)
        {

            // Our result
            var result = new CommandResultBase();

            // Generic failure message
            const string error = "Unauthorized";

            // IsHidden
            if (entity.IsHidden)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Articles.Permissions.ViewHiddenArticles))
                {
                    return result.Failed(error);
                }
            }

            // IsPrivate
            if (entity.IsPrivate)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Articles.Permissions.ViewPrivateArticles))
                {
                    return result.Failed(error);
                }
            }

            // IsSpam
            if (entity.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Articles.Permissions.ViewSpamArticles))
                {
                    return result.Failed(error);
                }
            }

            // IsDeleted
            if (entity.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Articles.Permissions.ViewDeletedArticles))
                {
                    return result.Failed(error);
                }
            }

            return result.Success();

        }

    }

}
