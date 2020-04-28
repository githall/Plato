using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using Plato.Roles.ViewModels;
using PlatoCore.Stores.Roles;
using PlatoCore.Models.Roles;
using PlatoCore.Data.Abstractions;
using Plato.Files.Extensions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Files.ViewProviders
{
    public class FileSettingsViewProvider : ViewProviderBase<FileSetting>
    {

        public static readonly long[] SizesInBytes = new long[] {        
            1048576,
            2097152,
            3145728,
            4194304,
            5242880,
            10485760,
            20971520,
            31457280,
            41943040,
            52428800,
            62914560,
            73400320,
            83886080,
            94371840,
            104857600,
            209715200,
            314572800,
            419430400,
            524288000,
            1073741824,
            2147483648,
            4294967296,
            8589934592,
            17179869184,
            34359738368,
            68719476736,
            137438953472,
            274877906944
        };

        private readonly IFileSettingsStore<FileSettings> _attachmentSettingsStore;        
        private readonly ILogger<FileSettingsViewProvider> _logger;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly HttpRequest _request;

        public const string ExtensionHtmlName = "extension";

        public FileSettingsViewProvider(
            IFileSettingsStore<FileSettings> attachmentSettingsStore,
            IHttpContextAccessor httpContextAccessor,     
            IPlatoRoleStore platoRoleStore,
            ILogger<FileSettingsViewProvider> logger,
            IShellSettings shellSettings,     
            IPlatoHost platoHost)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _attachmentSettingsStore = attachmentSettingsStore;            
            _platoRoleStore = platoRoleStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(FileSetting settings, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(FileSettingsViewModel)] as FileSettingsViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(FileSettingsViewModel).ToString()} has not been registered on the HttpContext!");
            }

            viewModel.Results = await GetRoles(viewModel.Options, viewModel.Pager);

            return Views(
                View<FileSettingsViewModel>("Admin.Settings.Header", model => viewModel).Zone("header").Order(1),
                View<FileSettingsViewModel>("Admin.Settings.Tools", model => viewModel).Zone("tools").Order(1),
                View<FileSettingsViewModel>("Admin.Settings.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(FileSetting settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(FileSetting setting, IViewProviderContext context)
        {

            var role = context.Controller.HttpContext.Items[typeof(Role)] as Role;
            if (role == null)
            {
                throw new Exception($"A view model of type {typeof(Role).ToString()} has not been registered on the HttpContext!");
            }

            // Defaults
            var maxFileSize = DefaultFileOptions.MaxFileSize;
            long availableSpace = DefaultFileOptions.AvailableSpace;
            var allowedExtensions = DefaultFileOptions.AllowedExtensions;

            // Populate settings
            var settings = await _attachmentSettingsStore.GetByRoleIdAsync(role.Id);
            if (settings != null)
            {
                maxFileSize = settings.MaxFileSize;
                availableSpace = settings.AvailableSpace;
                allowedExtensions = settings.AllowedExtensions;
            }

            // Build model
            var viewModel = new EditFileSettingsViewModel()
            {
                RoleId = role.Id,
                Role = role,
                MaxFileSize = maxFileSize,
                AvailableSpace = availableSpace,
                AvailableSpaces = GetAvailableSpaces(),
                DefaultExtensions = DefaultExtensions.Extensions,
                ExtensionHtmlName = ExtensionHtmlName,
                AllowedExtensions = allowedExtensions
            };

            // Build view
            return Views(
                View<EditFileSettingsViewModel>("Admin.EditSettings.Header", model => viewModel).Zone("header").Order(1),
                View<EditFileSettingsViewModel>("Admin.EditSettings.Tools", model => viewModel).Zone("tools").Order(1),
                View<EditFileSettingsViewModel>("Admin.EditSettings.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(FileSetting settings, IViewProviderContext context)
        {

            var model = new EditFileSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                var role = await _platoRoleStore.GetByIdAsync(model.RoleId);

                settings = new FileSetting()
                {
                    RoleId = role.Id,
                    MaxFileSize = model.MaxFileSize,
                    AvailableSpace = model.AvailableSpace,
                    AllowedExtensions = GetPostedExtensions()
                };

                var result = await _attachmentSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        // -----------------------

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

        string[] GetPostedExtensions()
        {

            if (!_request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var extensions = new List<string>();
            foreach (var key in _request.Form?.Keys)
            {
                if (key == ExtensionHtmlName)
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (!String.IsNullOrEmpty(value))
                        {
                            if (!extensions.Contains(value))                            
                                extensions.Add(value);
                        }
                    }
                }
            }

            return extensions.ToArray();

        }

        IEnumerable<SelectListItem> GetAvailableSpaces()
        {

            var output = new List<SelectListItem>();
            foreach (var size in SizesInBytes)
            {
                output.Add(new SelectListItem
                {
                    Text = size.ToFriendlyFileSize(0),
                    Value = size.ToString()
                });
            }

            return output;

        }

    }

}
