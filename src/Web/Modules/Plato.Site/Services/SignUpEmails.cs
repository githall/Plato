using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Site.Models;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;
using PlatoCore.Models.Users;

namespace Plato.Site.Services
{

    public interface ISignUpEmails
    {

        Task<ICommandResult<EmailMessage>> SendSecurityTokenAsync(SignUp signUp);

        Task<ICommandResult<EmailMessage>> SendEmailConfirmationTokenAsync(SignUp signUp);

    }

    public class SignUpEmails : ISignUpEmails
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;

        public SignUpEmails(
            IContextFacade contextFacade,
            ILocaleStore localeStore,
            IEmailManager emailManager)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
        }

        public async Task<ICommandResult<EmailMessage>> SendSecurityTokenAsync(SignUp signUp)
        {

            // Get reset password email
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "SignUpSecurityToken");
            if (email != null)
            {

                var subject = string.Format(email.Subject, signUp.SecurityToken);
                var body = string.Format(email.Message, signUp.SecurityToken);

                var message = new MailMessage()
                {
                    Subject = subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(signUp.Email);

                // send email
                return await _emailManager.SaveAsync(message);

            }

            var result = new CommandResult<EmailMessage>();
            return result.Failed("An error occurred whilst attempting to send the security token email.");

        }


        public async Task<ICommandResult<EmailMessage>> SendEmailConfirmationTokenAsync(SignUp signUp)
        {

            // Get confirm account email
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
            if (email != null)
            {            

                var body = string.Format(email.Message, signUp.SecurityToken);

                var message = new MailMessage()
                {
                    Subject = email.Subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(signUp.Email);

                // send email
                return await _emailManager.SaveAsync(message);

            }

            var result = new CommandResult<EmailMessage>();
            return result.Failed("An error occurred whilst attempting to send the account confirmation email.");

        }


    }

}
