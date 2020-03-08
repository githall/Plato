using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Attachments.Models;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.ViewModels;
using Plato.Entities.Models;

namespace Plato.Entities.Attachments.ViewComponents
{

    public class EntityAttachmentsViewComponent : ViewComponent
    {

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;

        public EntityAttachmentsViewComponent(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore)
        {
            _entityAttachmentStore = entityAttachmentStore;       
        }

        public async Task<IViewComponentResult> InvokeAsync(IEntity entity, IEntityReply reply)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var results = await _entityAttachmentStore.GetByEntityIdAsync(entity.Id);

            return View(new EntityAttachmentsViewModel()
            {
                Results = results
            });

        }

    }

}
