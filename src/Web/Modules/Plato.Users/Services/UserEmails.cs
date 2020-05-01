﻿using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Extensions;
using PlatoCore.Models.Users;

namespace Plato.Users.Services
{

    public interface IUserEmails
    {

        Task<ICommandResult<EmailMessage>> SendPasswordResetTokenAsync(IUser user);

        Task<ICommandResult<EmailMessage>> SendEmailConfirmationTokenAsync(IUser user);

    }

    public class UserEmails : IUserEmails
    {
        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;

        public UserEmails(
            IContextFacade contextFacade, 
            ILocaleStore localeStore, 
            IEmailManager emailManager)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
        }

        public async Task<ICommandResult<EmailMessage>> SendPasswordResetTokenAsync(IUser user)
        {

            // Get reset password email
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ResetPassword");
            if (email != null)
            {

                // Build reset password link
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                var callbackUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Users",
                    ["controller"] = "Account",
                    ["action"] = "ResetPassword",
                    ["code"] = user.ResetToken
                });

                var body = string.Format(email.Message, user.DisplayName, callbackUrl);

                var message = new MailMessage()
                {
                    Subject = email.Subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(user.Email);

                // send email
                return await _emailManager.SaveAsync(message);

            }

            var result = new CommandResult<EmailMessage>();
            return result.Failed("An error occurred whilst attempting to send the password reset token email.");

        }


        public async Task<ICommandResult<EmailMessage>> SendEmailConfirmationTokenAsync(IUser user)
        {

            // Get confirm account email
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
            if (email != null)
            {

                // Build email confirmation link
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                var callbackUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Users",
                    ["controller"] = "Account",
                    ["action"] = "ActivateAccount",
                    ["code"] = user.ConfirmationToken
                });

                var body = string.Format(email.Message, user.DisplayName, callbackUrl);

                var message = new MailMessage()
                {
                    Subject = email.Subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(user.Email);

                // send email
                return await _emailManager.SaveAsync(message);

            }

            var result = new CommandResult<EmailMessage>();
            return result.Failed("An error occurred whilst attempting to send the account confirmation email.");

        }


    }

}
