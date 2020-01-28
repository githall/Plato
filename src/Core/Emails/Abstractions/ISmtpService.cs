using System.Net.Mail;
using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace PlatoCore.Emails.Abstractions
{
    public interface ISmtpService
    {
        Task<ICommandResult<MailMessage>> SendAsync(MailMessage message);

    }
    
}
