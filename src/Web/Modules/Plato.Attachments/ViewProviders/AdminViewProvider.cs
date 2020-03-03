using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Shell;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Security.Abstractions.Encryption;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Attachments.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<AttachmentSettings>
    {

        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore,            
            ILogger<AdminViewProvider> logger,            
            IOptions<PlatoOptions> platoOptions,
            IShellSettings shellSettings,     
            IPlatoHost platoHost)
        {
            _attachmentSettingsStore = attachmentSettingsStore;
            _platoOptions = platoOptions.Value;
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

                //// Encrypt the password
                //var username = model.SmtpSettings.UserName;
                //var password = string.Empty;
                //if (!string.IsNullOrWhiteSpace(model.SmtpSettings.Password))
                //{
                //    try
                //    {                        
                //        password = _encrypter.Encrypt(model.SmtpSettings.Password);
                //    }
                //    catch (Exception e)
                //    {
                //        if (_logger.IsEnabled(LogLevel.Error))
                //        {
                //            _logger.LogError($"There was a problem encrypting the SMTP server password. {e.Message}");
                //        }
                //    }
                //}

                //settings = new EmailSettings()
                //{
                //    SmtpSettings = new SmtpSettings()
                //    {
                //        DefaultFrom = model.SmtpSettings.DefaultFrom,
                //        Host = model.SmtpSettings.Host,
                //        Port = model.SmtpSettings.Port,
                //        UserName = username,
                //        Password = password,
                //        RequireCredentials = model.SmtpSettings.RequireCredentials,
                //        EnableSsl = model.SmtpSettings.EnableSsl,
                //        PollingInterval = model.SmtpSettings.PollInterval,
                //        BatchSize = model.SmtpSettings.BatchSize,
                //        SendAttempts = model.SmtpSettings.SendAttempts,
                //        EnablePolling = model.SmtpSettings.EnablePolling
                //    }
                //};

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

            return new AttachmentSettingsViewModel();


            //if (settings != null)
            //{
            //    return new EmailSettingsViewModel()
            //    {
            //        SmtpSettings = new SmtpSettingsViewModel()
            //        {
            //            DefaultFrom = _platoOptions.DemoMode
            //                ? "email@example.com"
            //                : settings.SmtpSettings.DefaultFrom,
            //            Host = _platoOptions.DemoMode
            //                ? "smtp.example.com"
            //                : settings.SmtpSettings.Host,                        
            //            Port = settings.SmtpSettings.Port,
            //            UserName = _platoOptions.DemoMode
            //                ? "email@example.com"
            //                : settings.SmtpSettings.UserName,
            //            Password = _platoOptions.DemoMode
            //                ? ""
            //                : settings.SmtpSettings.Password,
            //            RequireCredentials = settings.SmtpSettings.RequireCredentials,
            //            EnableSsl = settings.SmtpSettings.EnableSsl,
            //            PollInterval = settings.SmtpSettings.PollingInterval,
            //            BatchSize = settings.SmtpSettings.BatchSize,
            //            SendAttempts = settings.SmtpSettings.SendAttempts,
            //            EnablePolling = settings.SmtpSettings.EnablePolling
            //        }
            //    };
            //}

            //// return default settings
            //return new EmailSettingsViewModel()
            //{
            //    SmtpSettings = new SmtpSettingsViewModel()
            //};

        }

    }

}
