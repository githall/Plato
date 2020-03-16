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
using Plato.Docs.Models;
using Plato.Entities.Stores;
using PlatoCore.Layout.ModelBinding;
using Plato.Entities.Files.ViewModels;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Services;
using PlatoCore.Hosting.Abstractions;
using Plato.Entities.Services;

namespace Plato.Docs.Files.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {

        public const string ModuleId = "Plato.Docs.Files";

        private readonly IFileViewIncrementer<File> _fileViewIncrementer;
        private readonly IEntityFileStore<EntityFile> _entityFileStore;        
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityService<Doc> _entityService; 
        private readonly IFileStore<File> _fileStore;

        public HomeController(
            IFileViewIncrementer<File> fileViewIncrementer,
            IEntityFileStore<EntityFile> entityFileStore,            
            IAuthorizationService authorizationService,
            IEntityService<Doc> entityService,       
            IFileStore<File> fileStore)
        {
            _authorizationService = authorizationService;
            _fileViewIncrementer = fileViewIncrementer;
            _entityFileStore = entityFileStore;
            _entityService = entityService;        
            _fileStore = fileStore;
        }

        // ----------
        // Download
        // ----------

        [HttpGet, AllowAnonymous]
        public async Task Download(int id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DownloadDocFiles))
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
        public Task<IActionResult> Edit(EntityFileOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityFileOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostDocFiles;
            opts.DeleteOwnPermission = Permissions.DeleteOwnDocFiles;
            opts.DeleteAnyPermission = Permissions.DeleteAnyDocFiles;

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
        public Task<IActionResult> Preview(EntityFileOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityFileOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostDocFiles;
            opts.DeleteOwnPermission = Permissions.DeleteOwnDocFiles;
            opts.DeleteAnyPermission = Permissions.DeleteAnyDocFiles;

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

        async Task<bool> AuthorizeAsync(File file)
        {

            // Get all relationships for the file
            var relationships = await _entityFileStore
                .QueryAsync()
                .Select<EntityFileQueryParams>(q =>
                {
                    q.FileId.Equals(file.Id);
                })
                .ToList();

            if (relationships?.Data != null)
            {

                // Get all entities for relationships
                var entities = await _entityService
                    .ConfigureQuery(async q =>
                    {

                        // Get all entities associated with file
                        q.Id.IsIn(relationships.Data.Select(r => r.EntityId).ToArray());

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Docs.Permissions.ViewPrivateDocs))
                        {
                            q.HidePrivate.True();
                        }

                        // Hide hidden?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Docs.Permissions.ViewHiddenDocs))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Docs.Permissions.ViewSpamDocs))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Docs.Permissions.ViewDeletedDocs))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync();

                // If we have results we have permission to view 
                // at least one of the entities associted with the file
                if (entities?.Data != null)
                {
                    return true;
                }
            }

            return false;

        }

    }

}
