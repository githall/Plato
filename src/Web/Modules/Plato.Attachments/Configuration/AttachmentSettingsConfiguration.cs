using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;

namespace Plato.Attachments.Configuration
{

    public class AttachmentSettingsConfiguration : IConfigureOptions<AttachmentSettings>
    {

        private readonly IAttachmentSettingsStore<AttachmentSettings> _attachmentSettingsStore;
        private readonly ILogger<AttachmentSettingsConfiguration> _logger;   

        public AttachmentSettingsConfiguration(
            IAttachmentSettingsStore<AttachmentSettings> attachmentSettingsStore,            
            ILogger<AttachmentSettingsConfiguration> logger)
        {
            _attachmentSettingsStore = attachmentSettingsStore;     
            _logger = logger;
        }

        public void Configure(AttachmentSettings options)
        {

            var settings = _attachmentSettingsStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options = settings;
            }
            else
            {

            }

        }

    }

}
