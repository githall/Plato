﻿using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Ideas.Models;
using Plato.Ideas.Tags.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Ideas.Tags.ViewProviders
{

    public class IdeaViewProvider : ViewProviderBase<Idea>
    {

        private const string ModuleId = "Plato.Ideas";
        private const string TagsHtmlName = "tags";

        private readonly IEntityTagManager<EntityTag> _entityTagManager;        
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityStore<Idea> _entityStore;        
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;
        private readonly ITagManager<Tag> _tagManager;
        private readonly ITagStore<Tag> _tagStore;

        private readonly HttpRequest _request;

        public IdeaViewProvider(
            IEntityTagManager<EntityTag> entityTagManager,
            IEntityTagStore<EntityTag> entityTagStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Idea> entityStore,             
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            ITagManager<Tag> tagManager,
            ITagStore<Tag> tagStore)
        {

            _entityTagManager = entityTagManager;
            _entityTagStore = entityTagStore;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _tagManager = tagManager;
            _tagStore = tagStore;

            _request = httpContextAccessor.HttpContext.Request;

        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Idea viewModel, IViewProviderContext context)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get top 20 tags for feature
            var tags = await _tagStore.QueryAsync()
                .Take(20, false)
                .Select<TagQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            return Views(
                View<TagsViewModel<Tag>>("Idea.Tags.Index.Sidebar", model =>
                {
                    model.Tags = tags?.Data;
                    return model;
                }).Zone("content-right").Order(4)
            );

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Idea idea, IViewProviderContext context)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            return Views(
                View<EditEntityTagsViewModel>("Idea.Tags.Edit.Footer", model => new EditEntityTagsViewModel()
                {
                    HtmlName = TagsHtmlName,
                    FeatureId = feature?.Id ?? 0,
                    Permission = Permissions.PostIdeaCommentTags
                }).Zone("resize-content")
                    .Order(int.MaxValue)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Idea idea, IViewProviderContext context)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Ensures we persist the tag json between post backs
            var tagsJson = "";
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == TagsHtmlName)
                    {
                        tagsJson = _request.Form[key];
                    }
                }
            }
            else
            {

                var entityTags = await GetEntityTagsByEntityIdAsync(idea.Id);

                // Exclude replies
                var entityTagList = entityTags?.Where(t => t.EntityReplyId == 0).ToList();

                if (entityTagList?.Count > 0)
                {

                    // Get entity tags
                    var tags = await _tagStore.QueryAsync()
                        .Select<TagQueryParams>(q =>
                        {
                            q.Id.IsIn(entityTagList.Select(e => e.TagId).ToArray());
                        })
                        .OrderBy("Name")
                        .ToList();

                    List<TagApiResult> tagsToSerialize = null;
                    if (tags != null)
                    {
                        tagsToSerialize = new List<TagApiResult>();
                        foreach (var tag in tags.Data)
                        {
                            tagsToSerialize.Add(new TagApiResult()
                            {
                                Id = tag.Id,
                                Name = tag.Name
                            });
                        }
                    }

                    if (tagsToSerialize != null)
                    {
                        tagsJson = tagsToSerialize.Serialize();
                    }

                }

            }

            return Views(
                View<EditEntityTagsViewModel>("Idea.Tags.Edit.Footer", model => new EditEntityTagsViewModel()
                {
                    Tags = tagsJson,
                    HtmlName = TagsHtmlName,
                    FeatureId = feature?.Id ?? 0,
                    Permission = idea.Id == 0
                            ? Permissions.PostIdeaTags
                            : Permissions.EditIdeaTags
                }).Zone("content")
                    .Order(int.MaxValue)
            );

        }

        public override Task<bool> ValidateModelAsync(Idea idea, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Idea idea, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(idea.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(idea, context);
            }

            // Validate model
            if (await ValidateModelAsync(idea, context.Updater))
            {

                // Get selected tags
                var tagsToAdd = await GetTagsToAddAsync();

                // Build tags to remove
                var tagsToRemove = new List<EntityTag>();

                // Get all existing tags for entity
                var existingTags = await GetEntityTagsByEntityIdAsync(idea.Id);

                // Exclude replies
                var existingTagsList = existingTags?.Where(t => t.EntityReplyId == 0).ToList();
                
                // Iterate over existing tags reducing our tags to add
                if (existingTagsList != null)
                {
                    foreach (var entityTag in existingTagsList)
                    {
                        // Is our existing tag in our list of tags to add
                        var existingTag = tagsToAdd.FirstOrDefault(t => t.Id == entityTag.TagId);
                        if (existingTag != null)
                        {
                            tagsToAdd.Remove(existingTag);
                        }
                        else
                        {
                            // Entry no longer exist in tags so ensure it's removed
                            tagsToRemove.Add(entityTag);
                        }
                    }
                }
            
                // Remove entity tags
                foreach (var entityTag in tagsToRemove)
                {
                    var result = await _entityTagManager.DeleteAsync(entityTag);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                
                // Get authenticated user
                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // Add new entity tags
                foreach (var tag in tagsToAdd)
                {
                    var result = await _entityTagManager.CreateAsync(new EntityTag()
                    {
                        EntityId = idea.Id,
                        TagId = tag.Id,
                        CreatedUserId = user?.Id ?? 0,
                        CreatedDate = DateTime.UtcNow
                    });
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

            }

            return await BuildEditAsync(idea, context);

        }

        #endregion

        #region "Private Methods"

        async Task<List<TagBase>> GetTagsToAddAsync()
        {

            var tagsToAdd = new List<TagBase>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.Equals(TagsHtmlName))
                {
                    var value = _request.Form[key];
                    if (!String.IsNullOrEmpty(value))
                    {

                        var items = JsonConvert.DeserializeObject<IEnumerable<TagApiResult>>(value);
                        foreach (var item in items)
                        {

                            // Get existing tag if we have an identity
                            if (item.Id > 0)
                            {
                                var tag = await _tagStore.GetByIdAsync(item.Id);
                                if (tag != null)
                                {
                                    tagsToAdd.Add(tag);
                                }
                            }
                            else
                            {

                                // Does the tag already exist by name?
                                var existingTag = await _tagStore.GetByNameNormalizedAsync(item.Name.Normalize());
                                if (existingTag != null)
                                {
                                    tagsToAdd.Add(existingTag);
                                }
                                else
                                {
                                    // Create tag
                                    var newTag = await CreateTag(item.Name);
                                    if (newTag != null)
                                    {
                                        tagsToAdd.Add(newTag);
                                    }
                                }
                            }

                        }

                    }

                }

            }

            return tagsToAdd;

        }

        async Task<TagBase> CreateTag(string name)
        {

            // Current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add tags
            if (user == null)
            {
                return null;
            }

            // Get feature for tag
            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);
      
            // Create tag
            var result = await _tagManager.CreateAsync(new Tag()
            {
                FeatureId = feature?.Id ?? 0,
                Name = name,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.UtcNow
            });
            
            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }

        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return null for new entities
                return null;
            }
            
            return await _entityTagStore.GetByEntityIdAsync(entityId);

        }

        #endregion

    }

}
