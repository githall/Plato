﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using PlatoCore.Text.Abstractions;

namespace Plato.Categories.Services
{

    public class CategoryManager<TCategory> : ICategoryManager<TCategory> where TCategory : class, ICategory
    {
        
        private readonly ICategoryRoleStore<CategoryRole> _categoryRoleStore;
        private readonly ICategoryDataStore<CategoryData> _categoryDataStore;
        private readonly ICategoryStore<TCategory> _categoryStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IBroker _broker;

        public CategoryManager(
            ICategoryStore<TCategory> categoryStore,
            ICategoryRoleStore<CategoryRole> categoryRoleStore,
            ICategoryDataStore<CategoryData> categoryDataStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IPlatoRoleStore roleStore,
            IBroker broker)
        {
            _categoryStore = categoryStore;
            _categoryRoleStore = categoryRoleStore;
            _roleStore = roleStore;
            _contextFacade = contextFacade;
            _categoryDataStore = categoryDataStore;
            _broker = broker;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<ICommandResult<TCategory>> CreateAsync(TCategory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }
            
            model.CreatedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
            
            // Invoke CategoryCreating subscriptions
            foreach (var handler in _broker.Pub<TCategory>(this, "CategoryCreating"))
            {
                model = await handler.Invoke(new Message<TCategory>(model, this));
            }
            
            var result = new CommandResult<TCategory>();

            var category = await _categoryStore.CreateAsync(model);
            if (category != null)
            {
             
                // Invoke CategoryCreated subscriptions
                foreach (var handler in _broker.Pub<TCategory>(this, "CategoryCreated"))
                {
                    category = await handler.Invoke(new Message<TCategory>(category, this));
                }

                // Return success
                return result.Success(category);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the category"));
            
        }

        public async Task<ICommandResult<TCategory>> UpdateAsync(TCategory model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
         
            // Invoke CategoryUpdating subscriptions
            foreach (var handler in _broker.Pub<TCategory>(this, "CategoryUpdating"))
            {
                model = await handler.Invoke(new Message<TCategory>(model, this));
            }
            
            var result = new CommandResult<TCategory>();

            var category = await _categoryStore.UpdateAsync(model);
            if (category != null)
            {

                // Invoke CategoryUpdated subscriptions
                foreach (var handler in _broker.Pub<TCategory>(this, "CategoryUpdated"))
                {
                    model = await handler.Invoke(new Message<TCategory>(model, this));
                }
                
                // Return success
                return result.Success(category);
            }

            return result.Failed("An unknown error occurred whilst attempting to update the category");
            
        }

        public async Task<ICommandResult<TCategory>> DeleteAsync(TCategory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Publish CategoryDeleting event
            foreach (var handler in _broker.Pub<TCategory>(this, "CategoryDeleting"))
            {
                model = await handler.Invoke(new Message<TCategory>(model, this));
            }

            var result = new CommandResult<TCategory>();
            if (await _categoryStore.DeleteAsync(model))
            {
                // Delete category roles
                await _categoryRoleStore.DeleteByCategoryIdAsync(model.Id);

                // Delete category data
                var data = await _categoryDataStore.GetByCategoryIdAsync(model.Id);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _categoryDataStore.DeleteAsync(item);
                    }
                    
                }

                // Publish CategoryDeleted event
                foreach (var handler in _broker.Pub<TCategory>(this, "CategoryDeleted"))
                {
                    model = await handler.Invoke(new Message<TCategory>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the category"));
            
        }

        public async Task<ICommandResult<TCategory>> AddToRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var result = new CommandResult<TCategory>();

            // Ensure the role exists
            var existingRole = await _roleStore.GetByNameAsync(roleName);
            if (existingRole == null)
            {
                return result.Failed(new CommandError($"A role with the name {roleName} could not be found"));
            }

            // Ensure supplied role name is not already associated with the category
            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return result.Failed(new CommandError($"A role with the name '{roleName}' is already associated with the category '{model.Name}'"));
                    }
                }
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
        
            var categoryRole = new CategoryRole()
            {
                CategoryId = model.Id,
                RoleId = existingRole.Id,
                CreatedUserId = user?.Id ?? 0
            };

            var updatedCategoryRole = await _categoryRoleStore.CreateAsync(categoryRole);
            if (updatedCategoryRole != null)
            {
                return result.Success();
            }

            return result.Failed(new CommandError($"An unknown error occurred whilst attempting to add role '{existingRole.Name}' for category '{model.Name}'"));

        }

        public async Task<ICommandResult<TCategory>> RemoveFromRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }
            
            // Our result
            var result = new CommandResult<TCategory>();
            
            // Ensure the role exists
            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return result.Failed(new CommandError($"A role with the name {roleName} could not be found"));
            }
            
            // Attempt to delete the role relationship
            var success = await _categoryRoleStore.DeleteByRoleIdAndCategoryIdAsync(role.Id, model.Id);
            if (success)
            {
                return result.Success();
            }
            
            return result.Failed(new CommandError($"An unknown error occurred whilst attempting to remove role '{role.Name}' for category '{model.Name}'"));

        }

        public async Task<bool> IsInRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }
            
            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles == null)
            {
                return false;
            }

            foreach (var localRole in roles)
            {
                if (localRole.RoleId == role.Id)
                {
                    return true;
                }
            }

            return false;

        }

        public async Task<IEnumerable<string>> GetRolesAsync(TCategory model)
        {

            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles != null)
            {
                return roles.Select(s => s.RoleName).ToArray();
            }

            return new string[] {};

        }

        public async Task<ICommandResult<TCategory>> Move(TCategory model, MoveDirection direction)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Our result
            var result = new CommandResult<TCategory>();

            // Add sort order to local variable as the model may change
            var sortOrder = model.SortOrder;

            // All categories for supplied category feature
            var categories = await _categoryStore.GetByFeatureIdAsync(model.FeatureId);
            if (categories == null)
            {
                return result.Failed($"No categories was found matching FeatureId '{model.FeatureId}'");
            }
            
            List<TCategory> children = null;
            switch (direction)
            {

                case MoveDirection.Up:

                    // Find category above the supplied category
                    TCategory above = null;
                    foreach (var category in categories.Where(c => c.ParentId == model.ParentId))
                    {
                        if (category.SortOrder < sortOrder)
                        {
                            above = (TCategory)category;
                        }
                    }

                    // Swap sort orders
                    if (above != null)
                    {
                        var update1 = await UpdateSortOrder(model, above.SortOrder);
                        if (update1.Succeeded)
                        {
                            var update2 = await UpdateSortOrder(above, sortOrder);
                            if (!update2.Succeeded)
                            {
                                result.Failed(update2.Errors.ToArray());
                            }
                        }
                        else
                        {
                            result.Failed(update1.Errors.ToArray());
                        }
                        
                    }

                    break;

                case MoveDirection.Down:
                    
                    // Find category below the supplied category
                    TCategory below = null;
                     children = categories
                        .Where(c => c.ParentId == model.ParentId)
                        .ToList();
                    for (var i = children.Count - 1; i >= 0; i--)
                    {
                        if (children[i].SortOrder > sortOrder)
                        {
                            below = (TCategory)children[i];
                        }
                    }
                    
                    // Swap sort orders
                    if (below != null)
                    {
                        var update1 = await UpdateSortOrder(model, below.SortOrder);
                        if (update1.Succeeded)
                        {
                            var update2 = await UpdateSortOrder(below, sortOrder);
                            if (!update2.Succeeded)
                            {
                                result.Failed(update2.Errors.ToArray());
                            }
                        }
                        else
                        {
                            result.Failed(update1.Errors.ToArray());
                        }
                        
                    }

                    break;

                case MoveDirection.ToTop:

                    // Find entity at the top of the current level
                    TCategory top = null;
                    children = categories.Where(c => c.ParentId == model.ParentId).ToList();
                    for (var i = children.Count - 1; i >= 0; i--)
                    {
                        top = (TCategory)children[i];
                    }

                    // Ensure we found the entity and we are not attempting to move
                    // the entity if it's already the top most entity
                    if (top != null && top.Id != model.Id)
                    {
                        var update = await UpdateSortOrder(model, top.SortOrder - 1);
                        if (!update.Succeeded)
                        {
                            result.Failed(update.Errors.ToArray());
                        }
                    }

                    break;

                case MoveDirection.ToBottom:

                    // Find entity at the bottom of the current level
                    TCategory bottom = null;
                    foreach (var entity in categories.Where(c => c.ParentId == model.ParentId))
                    {
                        bottom = (TCategory)entity;
                    }

                    // Ensure we found the entity and we are not attempting to move
                    // the entity if it's already the bottom most entity
                    if (bottom != null && bottom.Id != model.Id)
                    {
                        var update = await UpdateSortOrder(model, bottom.SortOrder + 1);
                        if (!update.Succeeded)
                        {
                            result.Failed(update.Errors.ToArray());
                        }
                    }

                    break;

            }

            return result.Success();

        }
        
        #endregion

        #region "Private Methods"
        
        async Task<ICommandResult<TCategory>> UpdateSortOrder(TCategory model, int sortOrder)
        {
           
            // Create result
            var result = new CommandResult<TCategory>();

            // Update sort order
            model.SortOrder = sortOrder;

            // Persist changes
            var updatedEntity = await _categoryStore.UpdateAsync(model);
            if (updatedEntity != null)
            {
                return result.Success(updatedEntity);
            }

            return result.Failed(
                $"An error occurred updating the sort order for category '{model.Name}' with Id '{model.Id}'.");


        }

        async Task<string> ParseAlias(string input)
        {

            foreach (var handler in _broker.Pub<string>(this, "ParseCategoryAlias"))
            {
                return await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return _aliasCreator.Create(input);

        }

        #endregion

    }

}
