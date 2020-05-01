﻿using System;
using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tenants.ViewModels;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using System.Collections.Generic;
using Plato.Tenants.Models;
using Plato.Tenants.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Security.Abstractions.Encryption;

namespace Plato.Tenants.ViewProviders
{

    public class AdminViewProvider : ViewProviderBase<ShellSettings>
    {

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly ITenantSetUpService _tenantSetUpService;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IEncrypter _encrypter;

        private readonly DefaultTenantSettings _defaultTenantSettings;

        public AdminViewProvider(
            IOptions<DefaultTenantSettings> tenantSetings,
            IShellSettingsManager shellSettingsManager,                         
            ILogger<AdminViewProvider> logger,
            ITenantSetUpService setUpService,
            IEncrypter encrypter)
        {
            _defaultTenantSettings = tenantSetings.Value;
            _shellSettingsManager = shellSettingsManager;                
            _tenantSetUpService = setUpService;
            _encrypter = encrypter;
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

            var viewModel = context.Controller.HttpContext.Items[typeof(TenantIndexViewModel)] as TenantIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TenantIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            viewModel.Results = _shellSettingsManager.LoadSettings();

            return Task.FromResult(Views(
                View<TenantIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                View<TenantIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                View<TenantIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(ShellSettings settings, IViewProviderContext updater)
        {

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Build view model
            var viewModel = GetModel(settings);        
            return Task.FromResult(Views(
                View<EditTenantViewModel>("Admin.Edit.Header", model => viewModel).Zone("header"),
                View<EditTenantViewModel>("Admin.Edit.Meta", model => viewModel).Zone("meta"),
                View<EditTenantViewModel>("Admin.Edit.Content", model => viewModel).Zone("content"),
                View<EditTenantViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer"),
                View<EditTenantViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions")
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(ShellSettings settings, IViewProviderContext context)
        {

            var model = new EditTenantViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                // Ensure password
                if (model.SmtpSettings != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.SmtpSettings.Password))
                    {
                        try
                        {
                            model.SmtpSettings.Password = _encrypter.Encrypt(model.SmtpSettings.Password);
                        }
                        catch (Exception e)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError($"There was a problem encrypting the SMTP server password. {e.Message}");
                            }
                        }
                    }
                }

                var setupContext = new TenantSetUpContext()
                {
                    SiteName = model.SiteName,
                    Location = model.Location,
                    DatabaseProvider = "SqlClient",
                    DatabaseConnectionString = model.ConnectionString,
                    AdminUsername = model.UserName,
                    AdminEmail = model.Email,
                    AdminPassword = model.Password,
                    RequestedUrlHost = model.RequestedUrlHost,
                    RequestedUrlPrefix = model.RequestedUrlPrefix,
                    State = model.State,
                    OwnerId = model.OwnerId,
                    CreatedDate = model.IsNewTenant ? DateTimeOffset.Now : model.CreatedDate,
                    ModifiedDate = model.IsNewTenant ? model.ModifiedDate : DateTimeOffset.Now,
                    EmailSettings = new EmailSettings()
                    {
                        SmtpSettings = model.SmtpSettings
                    },
                    Errors = new Dictionary<string, string>()
                };

                if (!model.TablePrefixPreset)
                {
                    setupContext.DatabaseTablePrefix = model.TablePrefix;
                }

                // Install or update tenant
                var result = model.IsNewTenant
                    ? await _tenantSetUpService.InstallAsync(setupContext)
                    : await _tenantSetUpService.UpdateAsync(setupContext);

                // Report any errors
                if (!result.Succeeded)
                {

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        if (model.IsNewTenant)
                        {
                            _logger.LogInformation($"Set-up of tenant '{setupContext.SiteName}' failed with the following errors...");
                        }
                        else
                        {
                            _logger.LogInformation($"Update of tenant '{setupContext.SiteName}' failed with the following errors...");
                        }                        
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

            return await BuildEditAsync(settings, context);

        }

        #endregion

        #region "Private Methods"

        private EditTenantViewModel GetModel(ShellSettings settings)
        {

            // Configure defaults
            string userName = "admin",
                email = "admin@admin.com",
                password = "Pa$1n@aDyN";

            // Build view model
            EditTenantViewModel viewModel = null;
            if (string.IsNullOrEmpty(settings.Name))
            {
                viewModel = new EditTenantViewModel()
                {
                    ConnectionString = _defaultTenantSettings.ConnectionString,
                    TablePrefix = _defaultTenantSettings.TablePrefix,
                    UserName = userName,
                    Email = email,
                    Password = password,
                    PasswordConfirmation = password,
                    SmtpSettings = new SmtpSettings()
                    {
                        DefaultFrom = _defaultTenantSettings.SmtpSettings.DefaultFrom,
                        Host = _defaultTenantSettings.SmtpSettings.Host,
                        Port = _defaultTenantSettings.SmtpSettings.Port,
                        UserName = _defaultTenantSettings.SmtpSettings.UserName,
                        Password = _defaultTenantSettings.SmtpSettings.Password,
                        RequireCredentials = _defaultTenantSettings.SmtpSettings.RequireCredentials,
                        EnableSsl = _defaultTenantSettings.SmtpSettings.EnableSsl
                    },
                    IsNewTenant = true
                };
            }
            else
            {
                viewModel = new EditTenantViewModel()
                {
                    SiteName = settings.Name,
                    Location = settings.Location,
                    ConnectionString = settings.ConnectionString,
                    TablePrefix = settings.TablePrefix,
                    RequestedUrlHost = settings.RequestedUrlHost,
                    RequestedUrlPrefix = settings.RequestedUrlPrefix,
                    UserName = userName,
                    Email = email,
                    Password = password,
                    PasswordConfirmation = password,
                    State = settings.State,
                    OwnerId = settings.OwnerId,
                    CreatedDate = settings.CreatedDate,
                    ModifiedDate = settings.ModifiedDate,
                    AvailableTenantStates = GetAvailableTenantStates()
                };
            }

            return viewModel;

        }

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

        private IEnumerable<SelectListItem> GetAvailableTenantStates()
        {

            var output = new List<SelectListItem>();
            foreach (var z in Enum.GetValues(typeof(TenantState)))
            {
                output.Add(new SelectListItem
                {
                    Text = z.ToString(),
                    Value = z.ToString()
                });
            }

            return output;
        }

        #endregion

    }

}

