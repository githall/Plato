using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Attachments.Stores;
using Plato.Attachments.Models;
using PlatoCore.Hosting.Abstractions;
using Plato.Entities.Attachments.Stores;
using Plato.Entities.Attachments.Models;
using PlatoCore.Text.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Entities.Attachments.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Users;
using Microsoft.AspNetCore.Routing;

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
        private readonly IKeyGenerator _keyGenerator;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public ArticleViewProvider(
            IEntityAttachmentStore<EntityAttachment> entityAttachmentStore,
            IAttachmentStore<Attachment> attachmentStore,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Article> entityStore,            
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            IKeyGenerator keyGenerator)
        {            
            _request = httpContextAccessor.HttpContext.Request;
            _entityAttachmentStore = entityAttachmentStore;
            _authorizationService = authorizationService;
            _attachmentStore = attachmentStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _keyGenerator = keyGenerator;
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext context)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Article(), context);
            }

            var featureId = 0; 
            var entityId = entity.Id;
            var contentGuid = string.Empty;

            // Get authenticated user
            var user = context.Controller.HttpContext.Features[typeof(User)] as User;

            // Get current feature
            var moduleId = "Plato.Articles.Attachments";
            var feature = await _featureFacade.GetFeatureByIdAsync(moduleId);
            if (feature != null)
            {
                featureId = feature.Id;
            }

            // Use posted guid if available
            var postedGuid = PostedGuidValue();
            if (!string.IsNullOrEmpty(postedGuid))
            {
                contentGuid = postedGuid;
            } 
            else
            {
                // Create a new temporary 256 bit unique ASCII string
                var key = _keyGenerator.GenerateKey(o =>
                {
                    o.MaxLength = 32;
                    o.UniqueIdentifier = $"{user.Id.ToString()}-{entity.Id.ToString()}";
                });
                // Convert to 256 bit / 32 character hexadecimal string
                contentGuid = key.ToStream(Encoding.ASCII).ToMD5().ToHex();
            }

            return Views(
                View<EntityAttachmentOptions>("Attachments.Edit.Sidebar", model =>
                {

                    model.FeatureId = featureId;
                    model.EntityId = entityId;
                    model.Guid = contentGuid;
                    model.GuidHtmlName = GuidHtmlName;

                    model.PostPermission = Permissions.PostArticleAttachments;
                    model.DeleteOwnPermission = Permissions.DeleteOwnArticleAttachments;
                    model.DeleteAnyPermission = Permissions.DeleteAnyArticleAttachments;

                    model.PostRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Attachments",
                        ["controller"] = "Api",
                        ["action"] = "Post",
                        ["guid"] = contentGuid,
                        ["featureId"] = entityId
                    };

                    model.EditRoute = new RouteValueDictionary()
                    {
                        ["area"] = moduleId,
                        ["controller"] = "Home",
                        ["action"] = "Index",
                        ["opts.guid"] = contentGuid,
                        ["opts.entityId"] = entityId
                    };

                    model.PreviewRoute = new RouteValueDictionary()
                    {
                        ["area"] = moduleId,
                        ["controller"] = "Home",
                        ["action"] = "Preview",
                        ["opts.guid"] = contentGuid,
                        ["opts.entityId"] = entityId
                    };

                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext updater)
        {

            // We need to be authenticated to add attachments
            var user = await _contextFacade.GetAuthenticatedUserAsync();       
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

            // Ensure we have a temporary guid
            var postedGuid = PostedGuidValue();           
            if (string.IsNullOrEmpty(postedGuid))
            {
                return await BuildEditAsync(article, updater);
            }

            // Get attachments for temporary guid
            var attachments = await _attachmentStore
                .QueryAsync()
                .Select<AttachmentQueryParams>(q => q.ContentGuid.Equals(postedGuid))
                .ToList();

            // Create relationships
            List<int> attachmentIds = null;
            if (attachments?.Data != null)
            {
                attachmentIds = new List<int>();
                foreach (var attachment in attachments.Data)
                {
                    // Create a relationship for any attachment matching our guid
                    var relationship = await _entityAttachmentStore.CreateAsync(new EntityAttachment()
                    {
                        EntityId = entity.Id,
                        AttachmentId = attachment.Id,
                        CreatedUserId = user.Id
                    });
                    if (relationship != null)
                    {
                        attachmentIds.Add(relationship.AttachmentId);
                    }
                }
            }

            // Reset temporary guid for established relationships
            if (attachmentIds != null)
            {
                await _attachmentStore.UpdateContentGuidAsync(
                    attachmentIds.ToArray(), string.Empty);
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
