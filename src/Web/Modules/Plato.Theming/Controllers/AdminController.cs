﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout;
using Plato.Theming.Models;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Theming.Abstractions;
using Plato.Theming.ViewModels;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Theming.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<ThemeAdmin> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteThemeFileManager _themeFileManager;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ISiteThemeLoader _siteThemeLoader;
        private readonly IShellSettings _shellSettings;
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoFileSystem _fileSystem;
        private readonly IThemeCreator _themeCreator;
        private readonly IPlatoHost _platoHost;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,    
            IViewProviderManager<ThemeAdmin> viewProvider,
            IAuthorizationService authorizationService,
            ISiteThemeFileManager themeFileManager,
            ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            ISiteThemeLoader siteThemeLoader,
            IContextFacade contextFacade,
            IShellSettings shellSettings,
            IPlatoFileSystem fileSystem,
            IThemeCreator themeCreator,
            ISitesFolder sitesFolder,
            IPlatoHost platoHost,
            IAlerter alerter)
        {
            _authorizationService = authorizationService;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _themeFileManager = themeFileManager;
            _siteThemeLoader = siteThemeLoader;
            _shellSettings = shellSettings;
            _contextFacade = contextFacade;
            _themeCreator = themeCreator;
            _viewProvider = viewProvider;
            _fileSystem = fileSystem;
            _platoHost = platoHost;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // ------------
        // Index
        // ------------

        public async Task<IActionResult> Index()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageThemes))
            {
                return Unauthorized();
            }
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Themes"]);
            });
                     
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new ThemeAdmin(), this));
            
        }
        
        // ------------
        // Create
        // ------------

        public async Task<IActionResult> Create()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.CreateThemes))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Theming"], tags => tags
                        .Action("Index", "Admin", "Plato.Theming")
                        .LocalNav())
                    .Add(S["Add Theme"]);
            });

            // We need to pass along the featureId
            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new ThemeAdmin(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(CreateThemeViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.CreateThemes))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {

                // Add model state errors 
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

                // Return
                return RedirectToAction(nameof(Index));

            }
            
            // Create theme
            var result = _themeCreator.CreateTheme(viewModel.ThemeId, viewModel.Name);
            if (result.Succeeded)
            {

                // Execute view providers
               await _viewProvider.ProvideUpdateAsync(new ThemeAdmin()
               {
                   ThemeId = viewModel.ThemeId,
                   Path = viewModel.Name
               }, this);

                // Add confirmation
                _alerter.Success(T["Theme Added Successfully!"]);

                // Return
                return RedirectToAction(nameof(Index));

            }
            else
            {
                // Report any errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);

        }

        // ------------
        // Edit
        // ------------

        public async Task<IActionResult> Edit(string id, string path)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditThemes))
            {
                return Unauthorized();
            }

            // Get theme
            var theme = _siteThemeLoader
                .AvailableThemes.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            // Ensure we found the theme
            if (theme == null)
            {
                return NotFound();
            }
            
            // Get theme files for current theme and path
            var themeFile = _themeFileManager.GetFile(id, path);

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Themes"], theming => theming
                        .Action("Index", "Admin", "Plato.Theming")
                        .LocalNav());
             
                // Build parents
                var parents = _themeFileManager.GetParents(id, path);
                if (parents != null)
                {

                    builder.Add(S[theme.Name], home => home
                        .Action("Edit", "Admin", "Plato.Theming", new RouteValueDictionary()
                        {
                            ["id"] = theme.Id,
                            ["path"] = ""
                        }).LocalNav());

                    foreach (var parent in parents)
                    {

                        if (string.IsNullOrEmpty(parent.RelativePath))
                        {
                            // don't render root - handled above
                            continue;
                        }

                        if (parent.RelativePath.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Add(S[parent.Name], home => home 
                                .LocalNav());
                        }
                        else
                        {
                            builder.Add(S[parent.Name], home => home
                                .Action("Edit", "Admin", "Plato.Theming", new RouteValueDictionary()
                                {
                                    ["id"] = theme.Id,
                                    ["path"] = parent.RelativePath
                                }).LocalNav());
                        }
               
                    }
                }
                else
                {
                    builder.Add(S[theme.Name], home => home
                        .LocalNav());
                }

            });
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new ThemeAdmin()
            {
                ThemeId = id,
                Path = path
            }, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditThemeViewModel model)
        {


            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditThemes))
            {
                return Unauthorized();
            }
            
            var result = await _viewProvider.ProvideUpdateAsync(new ThemeAdmin(), this);

            if (!ModelState.IsValid)
            {

                // Add model state errors 
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

                // Return
                return RedirectToAction(nameof(Index));

            }

            // Get theme files for current theme and path
            var themeFile = _themeFileManager.GetFile(model.ThemeId, model.Path);

            if (themeFile == null)
            {
                return NotFound();
            }

            // Write file
            await _themeFileManager.SaveFileAsync(themeFile, model.FileContents);

            // Display Confirmation
            _alerter.Success(T["File Updated Successfully!"]);

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Theming",
                ["controller"] = "Admin",
                ["action"] = "Edit",
                ["id"] = model.ThemeId,
                ["path"] = model.Path
            }));

        }

        // ------------
        // Delete
        // ------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteThemes))
            {
                return Unauthorized();
            }
            
            // Get theme
            var theme = _siteThemeLoader
                .AvailableThemes.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            // Ensure we found the theme
            if (theme == null)
            {
                return NotFound();
            }
            
            // Is the theme we are deleting our current theme
            var currentTheme = await _contextFacade.GetCurrentThemeAsync();
            if (currentTheme.Equals(theme.FullPath, StringComparison.OrdinalIgnoreCase))
            {

                // Get settings 
                var settings = await _siteSettingsStore.GetAsync();

                // Clear theme, ensures we fallback to our default theme
                settings.Theme = "";
                
                // Save settings
                var updatedSettings = await _siteSettingsStore.SaveAsync(settings);
                if (updatedSettings != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }
                
            }

            // Delete the theme from the file system
            var result = _fileSystem.DeleteDirectory(theme.FullPath);
            if (result)
            {
                _alerter.Success(T["Theme Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T[$"Could not delete the theme at \"{theme.FullPath}\""]);
            }

            return RedirectToAction(nameof(Index));

        }

    }

}
