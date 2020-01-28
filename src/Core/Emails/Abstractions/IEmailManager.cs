using System.Net.Mail;
using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace PlatoCore.Emails.Abstractions
{ 
    public interface IEmailManager
    {
        Task<ICommandResult<EmailMessage>> SaveAsync(MailMessage message);

        Task<ICommandResult<MailMessage>> SendAsync(MailMessage message);

    }

}
