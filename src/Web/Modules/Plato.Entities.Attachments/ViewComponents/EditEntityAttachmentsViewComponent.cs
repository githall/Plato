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
using Plato.Attachments.Services;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Users;

namespace Plato.Entities.Attachments.ViewComponents
{

    public class EditEntityAttachmentsViewComponent : ViewComponent
    {

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;
        private readonly IAttachmentInfoStore<AttachmentInfo> _attachmentInfoStore;
        private readonly IAttachmentOptionsFactory _attachmentOptionsFactory;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly IHttpContextAccessor _httpContextAccessor;  

        public EditEntityAttachmentsViewComponent(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,     
            IAttachmentInfoStore<AttachmentInfo> attachmentInfoStore,
            IAttachmentOptionsFactory attachmentOptionsFactory,
            IAttachmentStore<Attachment> attachmentStore,
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
            return View(new AttachmentsViewModel()
            {
                Info = await _attachmentInfoStore.GetByUserIdAsync(user?.Id ?? 0),
                Options = await _attachmentOptionsFactory.GetOptionsAsync(user),
                Results = await GetResultsAsync(model),
                PostPermission = model.PostPermission
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
