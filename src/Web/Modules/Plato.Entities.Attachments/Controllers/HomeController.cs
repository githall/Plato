using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Attachments.Stores;
using Plato.Entities.Attachments.ViewModels;
using Plato.Attachments.Models;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ModelBinding;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.Models;

namespace Plato.Entities.Attachments.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly IContextFacade _contextFacade;

        public HomeController(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            IAttachmentStore<Attachment> attachmentStore,
            IContextFacade contextFacade)
        {
            _entityAttachmentStore = entityAttachmentStore;
            _attachmentStore = attachmentStore;
            _contextFacade = contextFacade;
        }

        // Share dialog

        public async Task<IActionResult> Index(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }
            
            // If we don't have an entity Id we need a temporary guid
            if (opts.EntityId <= 0)
            {
                if (string.IsNullOrEmpty(opts.Guid))
                {
                    throw new ArgumentNullException(nameof(opts.Guid));
                }
            }

            // Get attachments
            var results = await _attachmentStore
                .QueryAsync()
                .Select<AttachmentQueryParams>(async q =>
                {
                    if (!string.IsNullOrEmpty(opts.Guid))
                    {
                        q.ContentGuid.Equals(opts.Guid);
                    } 
                    else
                    {
                        if (opts.EntityId > 0)
                        {
                            var relaationships = await _entityAttachmentStore
                                .GetByEntityIdAsync(opts.EntityId);
                            if (relaationships != null)
                            {
                                q.Id.IsIn(relaationships.Select(r => r.AttachmentId).ToArray());
                            }
                        }                               
                    }                    
                })
                .ToList();

            // Return view
            return View(new EntityAttachmentsIndexViewModel()
            {
                Results = results
            });

        }

    }

}
