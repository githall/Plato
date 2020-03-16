using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Files.Extensions;
using Plato.Files.Models;
using PlatoCore.Models.Users;

namespace Plato.Files.Services
{

    public class FileOptionsFactory : IFileOptionsFactory
    {

        private readonly FileSettings _settings;

        public FileOptionsFactory(IOptions<FileSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task<FileOptions> GetOptionsAsync(IUser user)
        {

            if (user == null)
            {
                return Task.FromResult(new FileOptions());
            }

            return Task.FromResult(new FileOptions()
            {
                AllowedExtensions = _settings.GetAllowedExtensions(user),
                MaxFileSize = _settings.GetMaxFileSize(user),
                AvailableSpace = _settings.GetAvailableSpace(user)
            });

        }

    }

}
