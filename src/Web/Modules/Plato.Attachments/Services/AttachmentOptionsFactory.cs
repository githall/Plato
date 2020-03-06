using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Plato.Attachments.Extensions;
using Plato.Attachments.Models;
using PlatoCore.Models.Users;
using System.Threading.Tasks;

namespace Plato.Attachments.Services
{

    public class AttachmentOptionsFactory : IAttachmentOptionsFactory
    {

        private readonly IHttpContextAccessor _httpContextAccessor;   

        private readonly AttachmentSettings _settings;
        
        public AttachmentOptionsFactory(
             IHttpContextAccessor httpContextAccessor,
            IOptions<AttachmentSettings> settings)
        {
            _httpContextAccessor = httpContextAccessor;
            _settings = settings.Value;
        }

        public Task<AttachmentOptions> GetSettingsAsync()
        {

            var user = _httpContextAccessor.HttpContext.Features[typeof(User)] as User;            
            if (user != null)
            {
                return Task.FromResult(new AttachmentOptions()
                {
                    AllowedExtensions = _settings.GetAllowedExtensions(user),
                    MaxFileSize = _settings.GetMaxFileSize(user),
                    AvailableSpace = _settings.GetAvailableSpace(user)
                });
            }

            return Task.FromResult(default(AttachmentOptions));

        }

    }

}
