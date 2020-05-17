﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Data.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using PlatoCore.Stores.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<Role>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly IPermissionsManager<Permission> _permissionsManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;

        public AdminViewProvider(
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            IPermissionsManager<Permission> permissionsManager,
            IAuthorizationService authorizationService,            
            IPlatoRoleStore platoRoleStore,
            RoleManager<Role> roleManager,
            UserManager<User> userManager)
        {
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _permissionsManager = permissionsManager;            
            _platoRoleStore = platoRoleStore;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(Role role, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Role role, IViewProviderContext context)
        {

            var indexViewModel = context.Controller.HttpContext.Items[typeof(RolesIndexViewModel)] as RolesIndexViewModel;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(RolesIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            var viewModel = await GetPagedModel(
                indexViewModel?.Options,
                indexViewModel?.Pager);

            return Views(
                View<RolesIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                View<RolesIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("header-right"),
                View<RolesIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Role role, IViewProviderContext updater)
        {

            // Locate the role we are editing within our default roles
            var defaultRole = DefaultRoles.ToList()
                .FirstOrDefault(r => r.Equals(role.Name, StringComparison.OrdinalIgnoreCase));

            // Build model
            var editRoleViewModel = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name,
                Role = role,
                IsNewRole = await IsNewRole(role.Id),
                IsDefaultRole = defaultRole != null ? true : false,
                EnabledPermissions = await GetEnabledRolePermissionsAsync(role),
                CategorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync()
            };

            // Return view
            return Views(
                View<EditRoleViewModel>("Admin.Edit.Header", model => editRoleViewModel).Zone("header"),                
                View<EditRoleViewModel>("Admin.Edit.Content", model => editRoleViewModel).Zone("content"),
                View<EditRoleViewModel>("Admin.Edit.Footer", model => editRoleViewModel).Zone("footer"),
                View<EditRoleViewModel>("Admin.Edit.Actions", model => editRoleViewModel).Zone("actions")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Role role, IViewProviderContext context)
        {

            var model = new EditRoleViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(role, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                role.Name = model.RoleName?.Trim();
                
                var result = await _roleManager.CreateAsync(role);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(role, context);

        }

        #endregion

        #region "Private Methods"

        async Task<bool> IsNewRole(int roleId)
        {
            return roleId == 0 || await _roleManager.FindByIdAsync(roleId.ToString()) == null;
        }
        
        async Task<RolesIndexViewModel> GetPagedModel(
            RoleIndexOptions options,
            PagerOptions pager)
        {
            var roles = await GetRoles(options, pager);
            return new RolesIndexViewModel(
                roles,
                options,
                pager);
        }

        async Task<IPagedResults<Role>> GetRoles(
            RoleIndexOptions options,
            PagerOptions pager)
        {
            return await _platoRoleStore.QueryAsync()
                .Take(pager.Page, pager.Size, pager.CountTotal)
                .Select<RoleQueryParams>(q =>
                {
                    if (options.RoleId > 0)
                    {
                        q.Id.Equals(options.RoleId);
                    }
                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                })
                .OrderBy("ModifiedDate", OrderBy.Desc)
                .ToList();
        }

        async Task<IEnumerable<string>> GetEnabledRolePermissionsAsync(Role role)
        {

            // We can only obtain enabled permissions for existing roles
            // Return an empty list for new roles to avoid additional null checks
            if (role.Id == 0)
            {
                return new List<string>();
            }

            // Build a dummy principal
            var principal = await _claimsPrincipalFactory.CreateAsync(new User()
            {
                RoleNames = new List<string>()
                {
                    role.Name
                }
            });
            
            // Permissions grouped by feature
            var categorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync();

            // Get flat permissions list from categorized permissions
            var permissions = categorizedPermissions.SelectMany(x => x.Value);

            var result = new List<string>();
            foreach (var permission in permissions)
            {
                if (await _authorizationService.AuthorizeAsync(principal, permission))
                {
                    result.Add(permission.Name);
                }
            }

            return result;

        }

        #endregion

    }

}
