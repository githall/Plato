using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using Plato.Entities.Files.Models;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.ViewModels;
using PlatoCore.Data.Abstractions;
using Plato.Files.Services;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Users;

namespace Plato.Entities.Files.ViewComponents
{

    public class EditEntityFilesViewComponent : ViewComponent
    {

        private readonly IEntityFileStore<EntityFile> _entityAttachmentStore;
        private readonly IFileInfoStore<FileInfo> _attachmentInfoStore;
        private readonly IFileOptionsFactory _attachmentOptionsFactory;
        private readonly IFileStore<File> _attachmentStore;
        private readonly IHttpContextAccessor _httpContextAccessor;  

        public EditEntityFilesViewComponent(
            IEntityFileStore<EntityFile> entityAttachmentStore,     
            IFileInfoStore<FileInfo> attachmentInfoStore,
            IFileOptionsFactory attachmentOptionsFactory,
            IFileStore<File> attachmentStore,
            IHttpContextAccessor httpContextAccessor)
        {
            _attachmentOptionsFactory = attachmentOptionsFactory;
            _entityAttachmentStore = entityAttachmentStore;            
            _attachmentInfoStore = attachmentInfoStore;            
            _httpContextAccessor = httpContextAccessor;
            _attachmentStore = attachmentStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityAttachmentOptions model)
        {

            // We always need a model
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We always need a guid
            if (string.IsNullOrEmpty(model.Guid))
            {
                throw new ArgumentNullException(nameof(model.Guid));
            }

            // Get current authenticated user
            var user = _httpContextAccessor.HttpContext.Features[typeof(User)] as User;

            // Build model & return view
            return View(new FilesViewModel()
            {
                Info = await _attachmentInfoStore.GetByUserIdAsync(user?.Id ?? 0),
                Options = await _attachmentOptionsFactory.GetOptionsAsync(user),
                Results = await GetResultsAsync(model),
                DeleteRoute = model.DeleteRoute,
                PostPermission = model.PostPermission,
                DeleteOwnPermission = model.DeleteOwnPermission,
                DeleteAnyPermission = model.DeleteAnyPermission
            });

        }

        private async Task<IPagedResults<File>> GetResultsAsync(EntityAttachmentOptions model)
        {

            IEnumerable<EntityFile> relaationships = null;
            if (model.EntityId > 0)
            {
                relaationships = await _entityAttachmentStore
                    .GetByEntityIdAsync(model.EntityId);
            }

            return await _attachmentStore
                .QueryAsync()
                .Take(int.MaxValue, false)
                .Select<FileQueryParams>(q =>
                {
                    // Get attachments for entity                               
                    if (relaationships != null)
                    {
                        q.Id.IsIn(relaationships.Select(r => r.FileId).ToArray());
                    }

                    // Get attachments for guid
                    q.ContentGuid.Equals(model.Guid).Or();

                })
                .OrderBy("TotalViews", OrderBy.Desc)
                .ToList();

        }

    }

}
