using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using Plato.Entities.Attachments.Models;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.ViewModels;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Attachments.ViewComponents
{

    public class PreviewEntityAttachmentsViewComponent : ViewComponent
    {
        
        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;        
        private readonly ILogger<PreviewEntityAttachmentsViewComponent> _logger;    
        private readonly IAttachmentStore<Attachment> _attachmentStore;

        public PreviewEntityAttachmentsViewComponent(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            ILogger<PreviewEntityAttachmentsViewComponent> logger,     
            IAttachmentStore<Attachment> attachmentStore)
        {
            _entityAttachmentStore = entityAttachmentStore;      
            _attachmentStore = attachmentStore;           
            _logger = logger;
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

            var results = await GetResultsAsync(model);

            // Build model & return view
            return View(new AttachmentsViewModel()
            {
                Results = results,
                PostPermission = model.PostPermission,
                DeleteOwnPermission = model.DeleteOwnPermission,
                DeleteAnyPermission = model.DeleteAnyPermission
            });

        }

        private async Task<IPagedResults<Attachment>> GetResultsAsync(EntityAttachmentOptions model)
        {

            IEnumerable<EntityAttachment> relaationships = null;
            if (model.EntityId > 0)
            {
                relaationships = await _entityAttachmentStore
                    .GetByEntityIdAsync(model.EntityId);
            }

            return await _attachmentStore
                .QueryAsync()
                .Take(int.MaxValue, false)
                .Select<AttachmentQueryParams>(q =>
                {
                    // Get attachments for entity                               
                    if (relaationships != null)
                    {
                        q.Id.IsIn(relaationships.Select(r => r.AttachmentId).ToArray());
                    }

                    // Get attachments for guid
                    q.ContentGuid.Equals(model.Guid).Or();

                })
                .ToList();

        }

    }

}
