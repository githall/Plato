using System.Threading.Tasks;
using PlatoCore.Emails.Abstractions;

namespace Plato.Email.Services
{

    public interface IEmailSettingsManager
    {
        Task<EmailSettings> SaveAsync(EmailSettings siteSettings);
    }

}
