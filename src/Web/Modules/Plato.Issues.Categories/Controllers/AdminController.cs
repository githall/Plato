﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Navigation.Abstractions;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using PlatoCore.Abstractions;
using Plato.Issues.Categories.Models;
using Plato.Issues.Categories.ViewModels;
using PlatoCore.Layout;

namespace Plato.Issues.Categories.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<CategoryAdmin> _viewProvider;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<CategoryAdmin> viewProvider,
            ICategoryManager<Category> categoryManager,
            ICategoryStore<Category> categoryStore,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {

            _breadCrumbManager = breadCrumbManager;
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        // --------------
        // Manage Categories
        // --------------

        public async Task<IActionResult> Index(int id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            IEnumerable<CategoryAdmin> parents = null;
            if (id > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(id);
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Issues"], issues => issues
                        .Action("Index", "Admin", "Plato.Issues")
                        .LocalNav());
            
                if (parents == null)
                {
                    builder.Add(S["Categories"]);
                }
                else
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Issues.Categories", new RouteValueDictionary { ["Id"] = 0 })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        if (parent.Id != id)
                        {
                            builder.Add(S[parent.Name], categories => categories
                                .Action("Index", "Admin", "Plato.Issues.Categories", new RouteValueDictionary { ["Id"] = parent.Id })
                                .LocalNav()
                            );
                        }
                        else
                        {
                            builder.Add(S[parent.Name]);
                        }
                     
                    }
                }

            });

            // Get optional current category
            CategoryAdmin currentCategory = null;
            if (id > 0)
            {
                currentCategory = await _categoryStore.GetByIdAsync(id);
            }
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(currentCategory ?? new CategoryAdmin(), this));

        }

        // --------------
        // Create Category
        // --------------

        public async Task<IActionResult> Create(int id = 0)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Issues"], issues => issues
                        .Action("Index", "Admin", "Plato.Issues")
                        .LocalNav())
                    .Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Issues.Categories")
                        .LocalNav()
                    ).Add(S["Add Category"]);
            });
            
            // We need to pass along the featureId
            var feature =  await GetCurrentFeature();
         
            // Build view model
            var viewModel = new Category
            {
                ParentId = id,
                FeatureId = feature.Id
            };

            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(viewModel, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditCategoryViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return await Create(viewModel.Id);
            }

            var iconCss = viewModel.IconCss;
            if (!string.IsNullOrEmpty(iconCss))
            {
                iconCss = viewModel.IconPrefix + iconCss;
            }

            var feature = await GetCurrentFeature();
            var category =  new Category()
            {
                ParentId = viewModel.ParentId,
                FeatureId = feature.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                ForeColor = viewModel.ForeColor,
                BackColor = viewModel.BackColor,
                IconCss = iconCss
            };

            var result = await _categoryManager.CreateAsync(category);
            if (result.Succeeded)
            {

                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                _alerter.Success(T["Category Added Successfully!"]);

                return RedirectToAction(nameof(Index));

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);
            
        }

        // --------------
        // Edit Category
        // --------------

        public async Task<IActionResult> Edit(int id)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Issues"], issues => issues
                        .Action("Index", "Admin", "Plato.Issues")
                        .LocalNav())
                    .Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Issues.Categories")
                        .LocalNav()
                    ).Add(S["Edit Category"]);
            });
            
            var category = await _categoryStore.GetByIdAsync(id);
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(category, this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public  async Task<IActionResult> EditPost(int id)
        {

            var currentCategory = await _categoryStore.GetByIdAsync(id);
            if (currentCategory == null)
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideUpdateAsync(currentCategory, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Category Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Move Up / Down
        // --------------

        public async Task<IActionResult> MoveUp(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.Up);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> MoveDown(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.Down);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Move To Top / Move To Bottom
        // --------------

        public async Task<IActionResult> MoveToTop(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.ToTop);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> MoveToBottom(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.ToBottom);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Open / Close
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Open(string id)
        {

            var ok = int.TryParse(id, out var categoryId);
            if (!ok)
            {
                return NotFound();
            }

            // Get category
            var category = await _categoryStore.GetByIdAsync(categoryId);

            // Ensure category exists
            if (category == null)
            {
                return NotFound();
            }

            // Flag category as closed
            var details = category.GetOrCreate<CategoryDetails>();
            details.Closed = false;
            category.AddOrUpdate(details);

            // Delete
            var result = await _categoryManager.UpdateAsync(category);

            if (result.Succeeded)
            {
                _alerter.Success(T["Category Opened Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(string id)
        {

            var ok = int.TryParse(id, out var categoryId);
            if (!ok)
            {
                return NotFound();
            }

            // Get category
            var category = await _categoryStore.GetByIdAsync(categoryId);

            // Ensure category exists
            if (category == null)
            {
                return NotFound();
            }

            // Flag category as closed
            var details = category.GetOrCreate<CategoryDetails>();
            details.Closed = true;
            category.AddOrUpdate(details);

            // Delete
            var result = await _categoryManager.UpdateAsync(category);

            if (result.Succeeded)
            {
                _alerter.Success(T["Category Closed Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }

            }

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Delete
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            var ok = int.TryParse(id, out var categoryId);
            if (!ok)
            {
                return NotFound();
            }

            // Get category
            var category = await _categoryStore.GetByIdAsync(categoryId);

            // Ensure category exists
            if (category == null)
            {
                return NotFound();
            }

            // Compile any errors that may occur
            var errors = new List<CommandError>();

            // First delete all child categories
            var children = await _categoryStore.GetChildrenByIdAsync(category.Id);
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child.Id == category.Id)
                    {
                        continue;
                    }
                    var childResult = await _categoryManager.DeleteAsync(child);
                    if (childResult.Errors.Any())
                    {
                        errors.AddRange(childResult.Errors);
                        break; // Exit if we encounter any errors
                    }
                }
            }

            // Report any errors that may have occurred whilst deleting
            // child categories and return
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
                return RedirectToAction(nameof(Index));
            }

            // Next if everything was OK delete the target category 
            var result = await _categoryManager.DeleteAsync(category);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Deleted Successfully"]);
            }
            else
            {
                // Report any errors
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // ---------

        private async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Issues.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }

}
