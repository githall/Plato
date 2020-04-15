using System;
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
using Plato.Tenants.ViewModels;
using PlatoCore.Models.Shell;

namespace Plato.Tenants.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<ShellSettings>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly IPermissionsManager<Permission> _permissionsManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        
        public AdminViewProvider(
            UserManager<User> userManager,
            IPlatoRoleStore platoRoleStore,
            RoleManager<Role> roleManager,
            IPermissionsManager<Permission> permissionsManager, 
            IAuthorizationService authorizationService,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory)
        {

            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _permissionsManager = permissionsManager;
            _platoRoleStore = platoRoleStore;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(ShellSettings role, IViewProviderContext updater)
        {

            return Task.FromResult(
                Views(
                    View<ShellSettings>("Admin.Display.Header", model => role).Zone("header"),
                    View<ShellSettings>("Admin.Display.Meta", model => role).Zone("meta"),
                    View<ShellSettings>("Admin.Display.Content", model => role).Zone("content"),
                    View<ShellSettings>("Admin.Display.Footer", model => role).Zone("footer")
                ));

        }

        public override Task<IViewProviderResult> BuildIndexAsync(ShellSettings role, IViewProviderContext context)
        {

            var indexViewModel = context.Controller.HttpContext.Items[typeof(TenantIndexViewModel)] as TenantIndexViewModel;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TenantIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            //var viewModel = await GetPagedModel(
            //    indexViewModel?.Options,
            //    indexViewModel?.Pager);

            return Task.FromResult(Views(
                View<TenantIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header"),
                View<TenantIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools"),
                View<TenantIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(ShellSettings role, IViewProviderContext updater)
        {

            //// Locate the role we are editing within our default roles
            //var defaultRole = DefaultRoles.ToList()
            //    .FirstOrDefault(r => r.Equals(role.Name, StringComparison.OrdinalIgnoreCase));

            //// Build model
            //var editRoleViewModel = new EditRoleViewModel()
            //{
            //    Id = role.Id,
            //    RoleName = role.Name,
            //    Role = role,
            //    IsNewRole = await IsNewRole(role.Id),
            //    IsDefaultRole = defaultRole != null ? true : false,
            //    EnabledPermissions = await GetEnabledRolePermissionsAsync(role),
            //    CategorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync()
            //};

            //// Return view
            //return Views(
            //    View<EditRoleViewModel>("Admin.Edit.Header", model => editRoleViewModel).Zone("header"),
            //    View<EditRoleViewModel>("Admin.Edit.Meta", model => editRoleViewModel).Zone("meta"),
            //    View<EditRoleViewModel>("Admin.Edit.Content", model => editRoleViewModel).Zone("content"),
            //    View<EditRoleViewModel>("Admin.Edit.Footer", model => editRoleViewModel).Zone("footer"),
            //    View<EditRoleViewModel>("Admin.Edit.Actions", model => editRoleViewModel).Zone("actions")
            //);


            return Task.FromResult(default(IViewProviderResult));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(ShellSettings role, IViewProviderContext context)
        {

            //var model = new EditRoleViewModel();

            //if (!await context.Updater.TryUpdateModelAsync(model))
            //{
            //    return await BuildEditAsync(role, context);
            //}

            //if (context.Updater.ModelState.IsValid)
            //{

            //    role.Name = model.RoleName?.Trim();
                
            //    var result = await _roleManager.CreateAsync(role);
            //    foreach (var error in result.Errors)
            //    {
            //        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
            //    }

            //}

            return await BuildEditAsync(role, context);

        }

        #endregion

        #region "Private Methods"

        #endregion

    }
}
