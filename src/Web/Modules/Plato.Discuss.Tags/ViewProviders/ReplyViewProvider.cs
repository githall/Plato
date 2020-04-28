using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class ReplyViewProvider : ViewProviderBase<Reply>
    {

        private const string ModuleId = "Plato.Discuss";
        private const string TagsHtmlName = "tags";

        private readonly IEntityTagManager<EntityTag> _entityTagManager;        
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityReplyStore<Reply> _replyStore;        
        private readonly ITagManager<TagBase> _tagManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;
        private readonly ITagStore<TagBase> _tagStore;

        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<Discuss.ViewProviders.TopicViewProvider> stringLocalize,
            IEntityTagManager<EntityTag> entityTagManager,
            IEntityTagStore<EntityTag> entityTagStore,
            IEntityReplyStore<Reply> replyStore,
            ITagManager<TagBase> tagManager,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            ITagStore<TagBase> tagStore)
        {

            _entityTagManager = entityTagManager;
            _entityTagStore = entityTagStore;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _replyStore = replyStore;
            _tagManager = tagManager;
            _tagStore = tagStore;

            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;

        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Reply reply, IViewProviderContext updater)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var tagsJson = "";
            var entityTags = await GetEntityTagsByEntityReplyIdAsync(reply.Id);
            if (entityTags != null)
            {

                var tags = await _tagStore.QueryAsync()
                    .Select<TagQueryParams>(q =>
                    {
                        q.Id.IsIn(entityTags.Select(e => e.TagId).ToArray());
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

            var viewModel = new EditEntityTagsViewModel()
            {
                Tags = tagsJson,
                HtmlName = TagsHtmlName,
                FeatureId = feature?.Id ?? 0,
                Permission = reply.Id == 0
                    ? Permissions.PostReplyTags
                    : Permissions.EditReplyTags
            };

            return Views(
                View<EditEntityTagsViewModel>("Topic.Tags.Edit.Footer", model => viewModel).Zone("content")
                    .Order(int.MaxValue)
            );


        }

        public override Task<bool> ValidateModelAsync(Reply reply, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {

            // Ensure entity reply exists before attempting to update
            var entity = await _replyStore.GetByIdAsync(reply.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Validate model
            if (await ValidateModelAsync(reply, context.Updater))
            {

                // Get selected tags
                var tagsToAdd = await GetTagsToAddAsync(reply);

                // Build tags to remove
                var tagsToRemove = new List<EntityTag>();

                // Iterate over existing tags
                var existingTags = await GetEntityTagsByEntityReplyIdAsync(reply.Id);
                if (existingTags != null)
                {
                    foreach (var entityTag in existingTags)
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

                // Add new entity labels
                foreach (var tag in tagsToAdd)
                {
                    var result = await _entityTagManager.CreateAsync(new EntityTag()
                    {
                        EntityId = reply.EntityId,
                        EntityReplyId = reply.Id,
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

            return await BuildEditAsync(reply, context);

        }

        #endregion

        #region "Private Methods"

        async Task<List<TagBase>> GetTagsToAddAsync(Reply reply)
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

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add tags
            if (user == null)
            {
                return null;
            }

            // Get feature for tag
            var feature = await _featureFacade.GetFeatureByIdAsync(ModuleId);

            // We always need a feature
            if (feature == null)
            {
                return null;
            }

            // Create tag
            var result = await _tagManager.CreateAsync(new TagBase()
            {
                FeatureId = feature.Id,
                Name = name,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            });

            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }

        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityReplyIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return null;
            }

            return await _entityTagStore.GetByEntityReplyIdAsync(entityId);

        }

        #endregion
        
    }

}
