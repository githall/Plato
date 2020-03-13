using Microsoft.Extensions.Options;
using Plato.Files.Models;
using Plato.Files.Stores;

namespace Plato.Files.Configuration
{

    public class AttachmentSettingsConfiguration : IConfigureOptions<FileSettings>
    {

        private readonly IFileSettingsStore<FileSettings> _attachmentSettingsStore;

        public AttachmentSettingsConfiguration(IFileSettingsStore<FileSettings> attachmentSettingsStore)
        {
            _attachmentSettingsStore = attachmentSettingsStore;             
        }

        public void Configure(FileSettings options)
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
