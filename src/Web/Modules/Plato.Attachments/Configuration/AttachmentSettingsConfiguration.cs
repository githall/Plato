using Microsoft.Extensions.Options;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;

namespace Plato.Attachments.Configuration
{

    public class AttachmentSettingsConfiguration : IConfigureOptions<AttachmentSettings>
    {

        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;

        public AttachmentSettingsConfiguration(IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore)
        {
            _attachmentSettingsStore = attachmentSettingsStore;             
        }

        public void Configure(AttachmentSettings options)
        {
            var settings = _attachmentSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.Settings = settings.Settings;                
            }

        }

    }

}
