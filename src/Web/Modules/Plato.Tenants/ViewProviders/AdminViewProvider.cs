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
using PlatoCore.Shell.Abstractions;

namespace Plato.Tenants.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<ShellSettings>
    {

        private readonly IShellSettingsManager _shellSettingsManager;

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly IPermissionsManager<Permission> _permissionsManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        
        public AdminViewProvider(
            IShellSettingsManager shellSettingsManager,
            UserManager<User> userManager,
            IPlatoRoleStore platoRoleStore,
            RoleManager<Role> roleManager,
            IPermissionsManager<Permission> permissionsManager, 
            IAuthorizationService authorizationService,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory)
        {

            _shellSettingsManager = shellSettingsManager;

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

            indexViewModel.Results = _shellSettingsManager.LoadSettings();

            return Task.FromResult(Views(
                View<TenantIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header"),
                View<TenantIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools"),
                View<TenantIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(ShellSettings model, IViewProviderContext updater)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

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


            var defaultConnectionString = "";



            EditTenantViewModel viewModel = null;
            if (string.IsNullOrEmpty(model.Name))
            {
                viewModel = new EditTenantViewModel()
                {                  
                    ConnectionString = "server=localhost;trusted_connection=true;database=plato",
                    TablePrefix = "plato",
                    UserName = "admin",
                    Email = "admin@admin.com",
                    Password = "admin",
                    PasswordConfirmation = "admin",
                    IsNewTenant = true
                };
            }
            else
            {
                viewModel = new EditTenantViewModel()
                {
                    SiteName = model.Name,
                    ConnectionString = model.ConnectionString,
                    TablePrefix = model.TablePrefix,              
                    IsNewTenant = false
                };
            }

            // Return view
            return Task.FromResult(Views(
                View<EditTenantViewModel>("Admin.Edit.Header", model => viewModel).Zone("header"),
                View<EditTenantViewModel>("Admin.Edit.Meta", model => viewModel).Zone("meta"),
                View<EditTenantViewModel>("Admin.Edit.Content", model => viewModel).Zone("content"),
                View<EditTenantViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer"),
                View<EditTenantViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions")
            ));


            //return Task.FromResult(default(IViewProviderResult));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(ShellSettings role, IViewProviderContext context)
        {

            var model = new EditTenantViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(role, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                //role.Name = model.RoleName?.Trim();

                //var result = await _roleManager.CreateAsync(role);
                //foreach (var error in result.Errors)
                //{
                //    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                //}

            }

            return await BuildEditAsync(role, context);

        }

        #endregion

        #region "Private Methods"

        #endregion

    }
}
