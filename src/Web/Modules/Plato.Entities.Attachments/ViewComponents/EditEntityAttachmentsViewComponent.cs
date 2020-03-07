using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using Plato.Entities.Attachments.Models;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.ViewModels;
using PlatoCore.Data.Abstractions;


namespace Plato.Entities.Attachments.ViewComponents
{

    public class EditEntityAttachmentsViewComponent : ViewComponent
    {

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;
        private readonly IAttachmentStore<Attachment> _attachmentStore;

        public EditEntityAttachmentsViewComponent(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            IAttachmentStore<Attachment> attachmentStore)
        {
            _entityAttachmentStore = entityAttachmentStore;
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

            // Get data & return view
            return View(new AttachmentsViewModel()
            {
                Results = await GetDataAsync(model)
            });

        }

        private async Task<IPagedResults<Attachment>> GetDataAsync(EntityAttachmentOptions model)
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
