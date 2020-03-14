using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using Plato.Entities.Files.Models;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.ViewModels;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Files.ViewComponents
{

    public class PreviewEntityFilesViewComponent : ViewComponent
    {

        private readonly IEntityFileStore<EntityFile> _entityAttachmentStore;        
        private readonly ILogger<PreviewEntityFilesViewComponent> _logger;    
        private readonly IFileStore<File> _attachmentStore;

        public PreviewEntityFilesViewComponent(
            IEntityFileStore<EntityFile> entityAttachmentStore,
            ILogger<PreviewEntityFilesViewComponent> logger,     
            IFileStore<File> attachmentStore)
        {
            _entityAttachmentStore = entityAttachmentStore;      
            _attachmentStore = attachmentStore;           
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityFileOptions model)
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

            // Build model & return view
            return View(new FilesViewModel()
            {
                Results = await GetResultsAsync(model),
                PostPermission = model.PostPermission,
                DeleteOwnPermission = model.DeleteOwnPermission,
                DeleteAnyPermission = model.DeleteAnyPermission
            });

        }

        private async Task<IPagedResults<File>> GetResultsAsync(EntityFileOptions model)
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
