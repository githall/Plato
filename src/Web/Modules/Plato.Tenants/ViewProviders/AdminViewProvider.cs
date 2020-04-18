﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using Plato.Tenants.ViewModels;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using System.Collections.Generic;
using Plato.Tenants.Models;
using Plato.Tenants.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Plato.Tenants.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<ShellSettings>
    {

        private readonly IShellSettingsManager _shellSettingsManager;
   
        private readonly IAuthorizationService _authorizationService;
        private readonly ITenantSetUpService _tenantSetUpService;
        private readonly ILogger<AdminViewProvider> _logger;

        public AdminViewProvider(
            IShellSettingsManager shellSettingsManager, 
            IAuthorizationService authorizationService,
            ILogger<AdminViewProvider> logger,
            ITenantSetUpService setUpService)
        {

            _shellSettingsManager = shellSettingsManager;        
            _authorizationService = authorizationService;         
            _tenantSetUpService = setUpService;
            _logger = logger;
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
                    RequestedUrlHost = model.RequestedUrlHost,
                    RequestedUrlPrefix = model.RequestedUrlPrefix
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

                var setupContext = new TenantSetUpContext()
                {
                    SiteName = model.SiteName,
                    DatabaseProvider = "SqlClient",
                    DatabaseConnectionString = model.ConnectionString,
                    AdminUsername = model.UserName,
                    AdminEmail = model.Email,
                    AdminPassword = model.Password,
                    RequestedUrlHost = model.RequestedUrlHost,
                    RequestedUrlPrefix = model.RequestedUrlPrefix,
                    Errors = new Dictionary<string, string>()
                };

                if (!model.TablePrefixPreset)
                {
                    setupContext.DatabaseTablePrefix = model.TablePrefix;
                }

                if (model.IsNewTenant)
                {

                    // Execute set-up
                    var result = await _tenantSetUpService.InstallAsync(setupContext);

                    // Report any errors
                    if (!result.Succeeded)
                    {

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation($"Set-up of tenant '{setupContext.SiteName}' failed with the following errors...");
                        }

                        foreach (var error in result.Errors)
                        {
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation(error.Code + " " + error.Description);
                            }
                            context.Updater.ModelState.AddModelError(error.Code, error.Description);
                        }

                    }

                }
                else
                {

                }

             


            }

            return await BuildEditAsync(role, context);

        }

        #endregion

        #region "Private Methods"

        private bool IsNewShell(string name)
        {

            var shells = _shellSettingsManager.LoadSettings();
            if (shells != null)
            {
                var shell = shells.First(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (shell != null)
                {
                    return false;
                }

            }

            return true;

        }

        #endregion

    }

}

