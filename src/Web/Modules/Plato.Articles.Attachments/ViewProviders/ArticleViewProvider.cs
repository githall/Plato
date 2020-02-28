using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;
using Plato.Entities.Attachments.ViewModels;
using Plato.Attachments.Stores;
using Plato.Attachments.Models;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.Models;

namespace Plato.Articles.Attachments.ViewProviders
{

    public class ArticleViewProvider : ViewProviderBase<Article>
    {

        private const string GuidHtmlName = "attachment-guid";     

        private readonly IEntityAttachmentStore<EntityAttachment> _entityAttachmentStore;
        private readonly IAttachmentStore<Attachment> _attachmentStore; 
        private readonly IAuthorizationService _authorizationService; 
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;

        public ArticleViewProvider(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            IAttachmentStore<Attachment> attachmentStore,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,        
            IEntityStore<Article> entityStore,
            IContextFacade contextFacade)
        {            
            _request = httpContextAccessor.HttpContext.Request;
            _entityAttachmentStore = entityAttachmentStore;
            _authorizationService = authorizationService;
            _attachmentStore = attachmentStore;
            _contextFacade = contextFacade;       
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {

            return Task.FromResult(default(IViewProviderResult));

            //if (entity == null)
            //{
            //    return await BuildIndexAsync(new Article(), updater);
            //}

            //var isFollowing = false;
            //var followType = FollowTypes.Article;

            //var user = await _contextFacade.GetAuthenticatedUserAsync();
            //if (user != null)
            //{
            //    var entityFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
            //        followType.Name,
            //        entity.Id,
            //        user.Id);
            //    if (entityFollow != null)
            //    {
            //        isFollowing = true;
            //    }
            //}
            
            //return Views(
            //    View<FollowViewModel>("Follow.Display.Tools", model =>
            //    {
            //        model.FollowType = followType;
            //        model.ThingId = entity.Id;
            //        model.IsFollowing = isFollowing;
            //        model.Permission = Permissions.FollowArticles;
            //        return model;
            //    }).Zone("tools").Order(-4)
            //);

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext context)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Article(), context);
            }

            var entityId = 0;
            var contentGuid = string.Empty;
            var user = await _contextFacade.GetAuthenticatedUserAsync();         

            // For new entities we need a temporary content guid
            if (entity.Id == 0)
            {
                var postedGuid = PostedGuidValue();

                // Use posted guid if available
                if (!string.IsNullOrEmpty(postedGuid))
                {
                    contentGuid = postedGuid;
                } 
                else
                {
                    // Create a new guid
                    contentGuid = System.Guid.NewGuid().ToString();
                }                
            }

            return Views(
                View<EntityAttachmentOptions>("Attachments.Edit.Sidebar", model =>
                {
                    model.EntityId = entityId;
                    model.Guid = contentGuid;
                    model.GuidHtmlName = GuidHtmlName;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext updater)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add attachments
            if (user == null)
            {
                return await BuildEditAsync(article, updater);
            }

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(article.Id);
            if (entity == null)
            {
                return await BuildEditAsync(article, updater);
            }

            // Get posted guid value
            var postedGuid = PostedGuidValue();

            // Ensure we have a guid
            if (!string.IsNullOrEmpty(postedGuid))
            {

                // Get attachments for guid
                var attachments = await _attachmentStore
                   .QueryAsync()
                   .Select<AttachmentQueryParams>(q =>
                   {
                       q.ContentGuid.Equals(postedGuid);
                   })
                    .ToList();

                if (attachments?.Data != null)
                {
                    foreach (var attachment in attachments.Data)
                    {
                        // Create a relationship for any attachment matching our guid
                        await _entityAttachmentStore.CreateAsync(new EntityAttachment()
                        {
                            EntityId = entity.Id,
                            AttachmentId = attachment.Id,
                            CreatedUserId = user.Id
                        });
                    }
                }

            }
           
            return await BuildEditAsync(article, updater);

        }

        string PostedGuidValue()
        {
            
            if (!_request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            foreach (var key in _request.Form?.Keys)
            {
                if (key == GuidHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        return _request.Form[key];
                    }
                }
            }
            return string.Empty;
        }

    }

}
