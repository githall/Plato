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
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.Services;
using Plato.Docs.Categories.ViewModels;
using Plato.Docs.Models;
using Plato.Docs.Services;
using Plato.Entities.Stores;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Docs.Categories.ViewProviders
{
    public class DocViewProvider : ViewProviderBase<Doc>
    {

        private const string CategoryHtmlName = "category";
        
        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Doc> _entityStore;        
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public DocViewProvider(
            IStringLocalizer stringLocalizer,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IEntityCategoryManager entityCategoryManager,
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IHttpContextAccessor httpContextAccessor,
            ICategoryStore<Category> categoryStore, 
            IBreadCrumbManager breadCrumbManager,
            IPostManager<Doc> entityManager,
            IEntityStore<Doc> entityStore,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityCategoryManager = entityCategoryManager;
            _categoryDetailsUpdater = categoryDetailsUpdater;
            _entityCategoryStore = entityCategoryStore;
            _breadCrumbManager = breadCrumbManager;
            _featureFacade = featureFacade;         
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _entityStore = entityStore;

            T = stringLocalizer;
            S = stringLocalizer;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Doc doc, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
                
            var categoryIndexViewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id,
                    CategoryId = doc.CategoryId
                }
            };

            return Views(
                View<CategoryIndexViewModel>("Doc.Categories.Index.Content", model => categoryIndexViewModel).Zone("content").Order(1),
                View<CategoryIndexViewModel>("Doc.Categories.Index.Sidebar", model => categoryIndexViewModel).Zone("sidebar").Order(int.MinValue + 10)
            );
            
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Doc doc, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base controller 
            IEnumerable<CategoryAdmin> parentCategories = null;
            if (doc.CategoryId > 0)
            {
                parentCategories = await _categoryStore.GetParentsByIdAsync(doc.CategoryId);
            }

            // Get parent entities
            var parentEntities = await _entityStore.GetParentsByIdAsync(doc.Id);
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Docs"], home => home
                    .Action("Index", "Home", "Plato.Docs")
                    .LocalNav()
                );

                // Parent categories
                if (parentCategories != null)
                {
                    builder.Add(S["Categories"], c => c
                        .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parentCategories)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }
                
                // Parent entities
                if (parentEntities != null)
                {
                    foreach (var parent in parentEntities)
                    {
                        if (parent.Id != doc.Id)
                        {
                            builder.Add(S[parent.Title], channel => channel
                                .Action("Display", "Home", "Plato.Docs", new RouteValueDictionary
                                {
                                    ["opts.id"] = parent.Id,
                                    ["opts.alias"] = parent.Alias,
                                })
                                .LocalNav()
                            );
                        }

                    }
                }
                
                builder.Add(S[doc.Title]);

            });
            
            // Build view
            var categoryIndexViewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id,
                    CategoryId = doc.CategoryId
                }
            };
            
            return Views(
                View<CategoryIndexViewModel>("Doc.Categories.Index.Sidebar", model => categoryIndexViewModel).Zone("sidebar").Order(int.MinValue + 10)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Doc doc, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs.Categories");

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Override breadcrumb configuration within base controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (doc.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(doc.CategoryId);
            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Docs"], home => home
                    .Action("Index", "Home", "Plato.Docs")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], channels => channels
                        .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], c => c
                            .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a topic title
                if (!String.IsNullOrEmpty(doc.Title))
                {
                    builder.Add(S[doc.Title], t => t
                        .Action("Display", "Home", "Plato.Docs", new RouteValueDictionary
                        {
                            ["opts.id"] = doc.Id,
                            ["opts.alias"] = doc.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[doc.Id > 0 ? "Edit Doc" : "New Doc"]);

            });
            
            var viewModel = new CategoryDropDownViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                },
                HtmlName = CategoryHtmlName,
                SelectedCategories = await GetCategoryIdsByEntityIdAsync(doc)
            };

            return Views(
                View<CategoryDropDownViewModel>("Doc.Categories.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(5)
            );

        }
        
        public override Task<bool> ValidateModelAsync(Doc doc, IUpdateModel updater)
        {
            // For docs, categories are optional, we may have docs at the root not within any category
            return Task.FromResult(true);
        }

        public override async Task ComposeModelAsync(Doc doc, IUpdateModel updater)
        {

            var model = new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd?.Count > 0)
                {
                    foreach (var categoryId in categoriesToAdd)
                    {
                        if (categoryId > 0)
                        {
                            doc.CategoryId = categoryId;
                        }
                    }
                }
                else
                {
                    doc.CategoryId = 0;
                }
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Doc doc, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(doc.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(doc, context);
            }

            // Validate model
            if (await ValidateModelAsync(doc, context.Updater))
            {
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();

                // Ensure we have categories to add
                if (categoriesToAdd?.Count > 0)
                {
                    
                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(doc))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(doc.Id, categoryId);
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
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(doc.Id, categoryId);
                        if (entityCategory == null)
                        {
                            // Add relationship
                            await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = doc.Id,
                                CategoryId = categoryId,
                                CreatedUserId = user?.Id ?? 0,
                                ModifiedUserId = user?.Id ?? 0,
                            });
                        }
                    }
                    
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
                else
                {

                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(doc))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(doc.Id, categoryId);
                        if (entityCategory != null)
                        {
                            await _entityCategoryManager.DeleteAsync(entityCategory);
                        }
                    }

                    // Update removed category meta data
                    foreach (var id in categoriesToRemove)
                    {
                        await _categoryDetailsUpdater.UpdateAsync(id);
                    }

                }

            }

            return await BuildEditAsync(doc, context);

        }

        #endregion

        #region "Private Methods"

        List<int> GetCategoriesToAdd()
        {

            // IMPORTANT: We always return a list here as the CategoryInputViewModel.SelectedCategories
            // property is [Required] but we don't always need to add docs to categories, for this
            // reason return an empty list to ensure ModelState validation passes even if no category is selected
            var categoriesToAdd = new List<int>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(CategoryHtmlName))
                {
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

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Doc entity)
        {

            // When creating a new entity use the categoryId set on the entity
            if (entity.Id == 0)
            {
                if (entity.CategoryId > 0)
                {
                    // return empty collection for new entities
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
