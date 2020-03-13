using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Files.Models;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.ViewModels;
using Plato.Entities.Models;

namespace Plato.Entities.Files.ViewComponents
{

    public class EntityFilesViewComponent : ViewComponent
    {

        private readonly IEntityFileStore<EntityFile> _entityAttachmentStore;

        public EntityFilesViewComponent(
            IEntityFileStore<EntityFile> entityAttachmentStore)
        {
            _entityAttachmentStore = entityAttachmentStore;       
        }

        public async Task<IViewComponentResult> InvokeAsync(IEntity entity, IEntityReply reply)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return View(new EntityFilesViewModel()
            {
                Results = await _entityAttachmentStore.GetByEntityIdAsync(entity.Id)
            });

        }

    }

}
