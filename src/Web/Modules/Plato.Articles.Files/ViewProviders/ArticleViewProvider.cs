using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Files.Stores;
using Plato.Files.Models;
using PlatoCore.Hosting.Abstractions;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Entities.Files.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Users;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Services;

namespace Plato.Articles.Files.ViewProviders
{

    public class ArticleViewProvider : ViewProviderBase<Article>
    {
        // Get current feature
        public const string ModuleId = "Plato.Articles.Files";

        private const string GuidHtmlName = "file-guid";     

        private readonly IEntityFileStore<EntityFile> _entityFileStore;
        private readonly IFileStore<File> _fileStore; 
        private readonly IFileGuidFactory _guidBuilder;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;        
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public ArticleViewProvider(
            IEntityFileStore<EntityFile> entityFileStore,
            IFileStore<File> fileStore,           
            IHttpContextAccessor httpContextAccessor,
            IFileGuidFactory guidBuilder,
            IEntityStore<Article> entityStore,            
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityFileStore = entityFileStore;          
            _fileStore = fileStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _guidBuilder = guidBuilder;
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

            var entityId = entity.Id;
            var contentGuid = string.Empty;

            // Get authenticated user
            var user = context.Controller.HttpContext.Features[typeof(User)] as User;

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);

            // Ensure the feature exists
            if (feature == null)
            {
                throw new Exception($"A feature named \"{ModuleId}\" could not be found!");
            }

            // Use posted guid if available
            var postedGuid = PostedGuidValue();
            if (!string.IsNullOrEmpty(postedGuid))
            {
                contentGuid = postedGuid;
            } 
            else
            {
                var uniqueKey = $"{user.Id.ToString()}-{entity.Id.ToString()}";
                contentGuid = _guidBuilder.NewGuid(uniqueKey);
            }

            return Views(
                View<EntityFileOptions>("Files.Edit.Sidebar", model =>
                {
                   
                    model.EntityId = entityId;
                    model.Guid = contentGuid;
                    model.GuidHtmlName = GuidHtmlName;

                    model.PostPermission = Permissions.PostArticleFiles;
                    model.DeleteOwnPermission = Permissions.DeleteOwnArticleFiles;
                    model.DeleteAnyPermission = Permissions.DeleteAnyArticleFile;

                    model.PostRoute = new RouteValueDictionary()
                    {
                        ["area"] = ModuleId,
                        ["controller"] = "Api",
                        ["action"] = "Post",
                        ["guid"] = contentGuid
                    };

                    model.EditRoute = new RouteValueDictionary()
                    {
                        ["area"] = ModuleId,
                        ["controller"] = "Home",
                        ["action"] = "Edit",
                        ["opts.guid"] = contentGuid,
                        ["opts.entityId"] = entityId
                    };

                    model.PreviewRoute = new RouteValueDictionary()
                    {
                        ["area"] = ModuleId,
                        ["controller"] = "Home",
                        ["action"] = "Preview",
                        ["opts.guid"] = contentGuid,
                        ["opts.entityId"] = entityId
                    };

                    return model;
                }).Zone("sidebar").Order(10)
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
            var attachments = await _fileStore
                .QueryAsync()
                .Select<FileQueryParams>(q => q.ContentGuid.Equals(postedGuid))
                .ToList();

            // Create relationships
            List<int> attachmentIds = null;
            if (attachments?.Data != null)
            {
                attachmentIds = new List<int>();
                foreach (var attachment in attachments.Data)
                {
                    // Create a relationship for any attachment matching our guid
                    var relationship = await _entityFileStore.CreateAsync(new EntityFile()
                    {
                        EntityId = entity.Id,
                        FileId = attachment.Id,
                        CreatedUserId = user.Id
                    });
                    if (relationship != null)
                    {
                        attachmentIds.Add(relationship.FileId);
                    }
                }
            }

            // Reset temporary guid for established relationships
            if (attachmentIds != null)
            {
                await _fileStore.UpdateContentGuidAsync(
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
