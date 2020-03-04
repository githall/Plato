using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Roles.ViewModels;
using PlatoCore.Stores.Abstractions.Roles;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Roles;

namespace Plato.Attachments.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<AttachmentSettings>
    {

        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly HttpRequest _request;

        public const string ExtensionHtmlName = "extension";

        public AdminViewProvider(
            IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore,
            IHttpContextAccessor httpContextAccessor,     
            IPlatoRoleStore platoRoleStore,
            ILogger<AdminViewProvider> logger,
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

        public override async Task<IViewProviderResult> BuildIndexAsync(AttachmentSettings settings, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(AttachmentsIndexViewModel)] as AttachmentsIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(AttachmentsIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            viewModel.Results = await GetRoles(viewModel.Options, viewModel.Pager);

            return Views(
                View<AttachmentsIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<AttachmentsIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<AttachmentsIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AttachmentSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(AttachmentSettings settings, IViewProviderContext context)
        {

            var viewModel = await GetModel();
            return Views(
                View<EditAttachmentSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<EditAttachmentSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<EditAttachmentSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AttachmentSettings settings, IViewProviderContext context)
        {

            var model = new AttachmentsIndexViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                settings = new AttachmentSettings()
                {
                    AllowedExtensions = GetPostedExtensions()
                };

                var result = await _attachmentSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        // -----------------------

        async Task<EditAttachmentSettingsViewModel> GetModel()
        {
            
            var settings = await _attachmentSettingsStore.GetAsync();
            return new EditAttachmentSettingsViewModel()
            {
                DefaultExtensions = DefaultExtensions.Extensions,
                ExtensionHtmlName = ExtensionHtmlName,           
                AllowedExtensions = settings != null
                    ? settings.AllowedExtensions
                    : DefaultExtensions.AllowedExtensions
            };

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

    }

}
