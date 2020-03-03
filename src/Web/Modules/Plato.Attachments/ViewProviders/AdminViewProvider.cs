using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Attachments.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<AttachmentSettings>
    {

        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly HttpRequest _request;

        public const string ExtensionHtmlName = "extension";

        public AdminViewProvider(
            IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore,
            IHttpContextAccessor httpContextAccessor,
            IOptions<PlatoOptions> platoOptions,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,     
            IPlatoHost platoHost)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _attachmentSettingsStore = attachmentSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(AttachmentSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AttachmentSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(AttachmentSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<AttachmentSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<AttachmentSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<AttachmentSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AttachmentSettings settings, IViewProviderContext context)
        {

            var model = new AttachmentSettingsViewModel();

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

        async Task<AttachmentSettingsViewModel> GetModel()
        {

            var settings = await _attachmentSettingsStore.GetAsync();
            return new AttachmentSettingsViewModel()
            {
                DefaultExtensions = DefaultExtensions.Extensions,
                ExtensionHtmlName = ExtensionHtmlName,
                AllowedExtensions = settings != null
                    ? settings.AllowedExtensions
                    : DefaultExtensions.AllowedExtensions
            };

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
