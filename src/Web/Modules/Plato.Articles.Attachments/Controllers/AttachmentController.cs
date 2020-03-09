using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PlatoCore.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Attachments.Stores;
using Plato.Entities.Models;
using Plato.Attachments.Models;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.Models;
using Plato.Articles.Models;
using Plato.Entities.Stores;

namespace Plato.Articles.Attachments.Controllers
{

    public class AttachmentController : Controller
    {

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;
        private readonly IAttachmentStore<Attachment> _attachmentStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Article> _entityStore;

        public AttachmentController(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            IAttachmentStore<Attachment> attachmentStore,
            IAuthorizationService authorizationService,
            IEntityStore<Article> entityStore)
        {
            _entityAttachmentStore = entityAttachmentStore;
            _authorizationService = authorizationService;
            _attachmentStore = attachmentStore;
            _entityStore = entityStore;
        }

        // ----------
        // Download
        // ----------

        [HttpGet, AllowAnonymous]
        public async Task Download(int id)
        {

            // Clear response
            var r = Response;
            r.Clear();
            
            // Get attachment
            var attachment = await _attachmentStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (attachment == null)
            {           
                return;
            }

            // Do we have permission to view at least one of the
            // entities the attachment is associated with
            if (!await AuthorizeAsync(attachment))
            {              
                return;
            }
            
            if (attachment.ContentLength <= 0)
            {
                return;
            }

            // Update total views
            attachment.TotalViews += 1;

            // Update attachment
            await _attachmentStore.UpdateAsync(attachment);

            _entityAttachmentStore.CancelTokens(null);

            // Serve attachment         
            r.ContentType = attachment.ContentType;
            r.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + attachment.Name + "\"");
            r.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)attachment.ContentLength));                
            await r.Body.WriteAsync(attachment.ContentBlob, 0, (int)attachment.ContentLength);            

        }

        // ----------
        // Delete
        // ----------

        [HttpGet, AllowAnonymous]
        public async Task Delete(int id)
        {

            // Clear response
            var r = Response;
            r.Clear();

            // Get attachment
            var attachment = await _attachmentStore.GetByIdAsync(id);

            // Ensure attachment exists
            if (attachment == null)
            {
                return;
            }

            // Do we have permission to view at least one of the
            // entities the attachment is associated with
            if (!await AuthorizeAsync(attachment))
            {
                return;
            }

            // Update total views
            attachment.TotalViews = attachment.TotalViews + 1;
            await _attachmentStore.UpdateAsync(attachment);

            if (attachment.ContentLength >= 0)
            {
                r.ContentType = attachment.ContentType;
                r.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + attachment.Name + "\"");
                r.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)attachment.ContentLength));
                await r.Body.WriteAsync(attachment.ContentBlob, 0, (int)attachment.ContentLength);
            }

        }

        // ------------------------------

        async Task<bool> AuthorizeAsync(Attachment attachment)
        {

            // Get all entities associated with the attachment
            var relationships = await _entityAttachmentStore
                .QueryAsync()
                .Select<EntityAttachmentQueryParams>(q =>
                {
                    q.AttachmentId.Equals(attachment.Id);
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