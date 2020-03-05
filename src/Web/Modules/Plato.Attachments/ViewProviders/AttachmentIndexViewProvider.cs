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
using Plato.Roles.ViewModels;
using PlatoCore.Stores.Roles;
using PlatoCore.Models.Roles;
using PlatoCore.Data.Abstractions;
using Plato.Attachments.Extensions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Attachments.ViewProviders
{
    public class AttachmentIndexViewProvider : ViewProviderBase<AttachmentIndex>
    {

      
        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;        
        private readonly ILogger<AttachmentSettingsViewProvider> _logger;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly HttpRequest _request;

        public const string ExtensionHtmlName = "extension";

        public AttachmentIndexViewProvider(
            IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore,
            IHttpContextAccessor httpContextAccessor,     
            IPlatoRoleStore platoRoleStore,
            ILogger<AttachmentSettingsViewProvider> logger,
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

        public override Task<IViewProviderResult> BuildIndexAsync(AttachmentIndex model, IViewProviderContext context)
        {

            // Get view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(AttachmentIndexViewModel)] as AttachmentIndexViewModel;

            // Ensure we have the view model
            if (viewModel == null)
            {
                throw new Exception($"No type of \"{typeof(AttachmentIndexViewModel)}\" has been registered with HttpContext.Items");
            }

            return Task.FromResult(Views(
                View<AttachmentIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<AttachmentIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<AttachmentIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AttachmentIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(AttachmentIndex model, IViewProviderContext context)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AttachmentIndex model, IViewProviderContext context)
        {
           
            return await BuildEditAsync(model, context);

        }

        // -----------------------    

    }

}
