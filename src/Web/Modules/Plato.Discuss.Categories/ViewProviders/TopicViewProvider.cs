﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Discuss.Categories.Models;
using Plato.Discuss.Categories.Services;
using Plato.Discuss.Categories.ViewModels;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities.Stores;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Categories.ViewProviders
{
    public class TopicViewProvider : ViewProviderBase<Topic>
    {

        private const string CategoryHtmlName = "category";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IPostManager<Topic> _entityManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }

        public TopicViewProvider(
            IStringLocalizer stringLocalizer,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IEntityCategoryManager entityCategoryManager,
            IHttpContextAccessor httpContextAccessor,
            ICategoryStore<Category> categoryStore, 
            IBreadCrumbManager breadCrumbManager,
            IPostManager<Topic> entityManager,
            IEntityStore<Topic> entityStore,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityCategoryManager = entityCategoryManager;
            _categoryDetailsUpdater = categoryDetailsUpdater;
            _entityCategoryStore = entityCategoryStore;
            _breadCrumbManager = breadCrumbManager;       
            _featureFacade = featureFacade;
            _entityManager = entityManager;
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _entityStore = entityStore;

            T = stringLocalizer;
            S = stringLocalizer;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic viewModel, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categoryIndexViewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                }
            };

            return Views(
                View<CategoryIndexViewModel>("Discuss.Categories.Sidebar", model => categoryIndexViewModel).Zone("sidebar")
                    .Order(1)
            );
            
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(topic.CategoryId);
            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], c => c
                        .Action("Index", "Home", "Plato.Discuss.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Discuss.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[topic.Title]);

            });

            return default(IViewProviderResult);

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Categories");

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (entity.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(entity.CategoryId);
            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], c => c
                            .Action("Index", "Home", "Plato.Discuss.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a topic title
                if (!String.IsNullOrEmpty(entity.Title))
                {
                    builder.Add(S[entity.Title], t => t
                        .Action("Display", "Home", "Plato.Discuss", new RouteValueDictionary
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[entity.Id > 0 ? "Edit Topic" : "New Topic"]);

            });
            
            var viewModel = new CategoryDropDownViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                },
                HtmlName = CategoryHtmlName,
                SelectedCategories = await GetCategoryIdsByEntityIdAsync(entity)
            };

            return Views(
                View<CategoryDropDownViewModel>("Discuss.Categories.Edit.Sidebar", model => viewModel)
                    .Zone("sidebar").Order(5)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeModelAsync(Topic topic, IUpdateModel updater)
        {

            var model = new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {
                    foreach (var categoryId in categoriesToAdd)
                    {
                        if (categoryId > 0)
                        {
                            topic.CategoryId = categoryId;
                        }
                    }
                }
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, context);
            }

            // Validate model
            if (await ValidateModelAsync(topic, context.Updater))
            {
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {
                    
                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(topic))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(topic.Id, categoryId);
                        if (entityCategory != null)
                        {
                            await _entityCategoryManager.DeleteAsync(entityCategory);
                        }
                    }

                    // Get current user
                    var user = await _contextFacade.GetAuthenticatedUserAsync();

                    // Add new entity category relationships
                    foreach (var categoryId in categoriesToAdd)
                    {
                        // Ensure relationship does not already exist
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(topic.Id, categoryId);
                        if (entityCategory == null)
                        {
                            // Add relationship
                            await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = topic.Id,
                                CategoryId = categoryId,
                                CreatedUserId = user?.Id ?? 0,
                                ModifiedUserId = user?.Id ?? 0,
                            });
                        }
                    }
                    
                    //// Update entity with first found category 
                    //foreach (var id in categoriesToAdd)
                    //{
                    //    topic.CategoryId = id;
                    //    await _entityStore.UpdateAsync(topic);
                    //    break;
                    //}
                    
                    // Update added category meta data
                    foreach (var id in categoriesToAdd)
                    {
                        await _categoryDetailsUpdater.UpdateAsync(id);
                    }

                    // Update removed category meta data
                    foreach (var id in categoriesToRemove)
                    {
                        await _categoryDetailsUpdater.UpdateAsync(id);
                    }

                }

            }
           
            return await BuildEditAsync(topic, context);

        }
        
        #endregion

        #region "Private Methods"
        
        List<int> GetCategoriesToAdd()
        {
            // Build selected categories
            List<int> categoriesToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(CategoryHtmlName))
                {
                    if (categoriesToAdd == null)
                    {
                        categoriesToAdd = new List<int>();
                    }
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (!categoriesToAdd.Contains(id))
                        {
                            categoriesToAdd.Add(id);
                        }
                    }
                }
            }

            return categoriesToAdd;
        }

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Topic entity)
        {

            // When creating a new topic use the categoryId set on the topic
            if (entity.Id == 0)
            {
                if (entity.CategoryId > 0)
                {
                    // return empty collection for new topics
                    return new List<int>()
                    {
                        entity.CategoryId
                    };
                }

                return new List<int>();

            }

            var categories = await _entityCategoryStore.GetByEntityIdAsync(entity.Id);;
            if (categories != null)
            {
                return categories.Select(s => s.CategoryId).ToArray();
            }

            return new List<int>();

        }

        #endregion

    }

}
