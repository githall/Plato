using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Attachments.Extensions;
using Plato.Attachments.Models;
using PlatoCore.Models.Users;

namespace Plato.Attachments.Services
{

    public class AttachmentOptionsFactory : IAttachmentOptionsFactory
    {

        private readonly AttachmentSettings _settings;

        public AttachmentOptionsFactory(IOptions<AttachmentSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task<AttachmentOptions> GetOptionsAsync(IUser user)
        {

            if (user == null)
            {
                return Task.FromResult(default(AttachmentOptions));
            }

            return Task.FromResult(new AttachmentOptions()
            {
                AllowedExtensions = _settings.GetAllowedExtensions(user),
                MaxFileSize = _settings.GetMaxFileSize(user),
                AvailableSpace = _settings.GetAvailableSpace(user)
            });

        }

    }

}
