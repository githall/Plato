using Microsoft.Extensions.Options;
using Plato.Files.Models;
using Plato.Files.Stores;

namespace Plato.Files.Configuration
{

    public class FileSettingsConfiguration : IConfigureOptions<FileSettings>
    {

        private readonly IFileSettingsStore<FileSettings> _fileSettingsStore;

        public FileSettingsConfiguration(IFileSettingsStore<FileSettings> fileSettingsStore)
        {
            _fileSettingsStore = fileSettingsStore;             
        }

        public void Configure(FileSettings options)
        {
            var settings = _fileSettingsStore
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
