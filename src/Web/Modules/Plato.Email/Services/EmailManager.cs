﻿using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Emails.Abstractions.Extensions;
using PlatoCore.Messaging.Abstractions;

namespace PlatoCore.Emails.Abstractions
{
    
    public class EmailManager : IEmailManager
    {

        private readonly IEmailAttachmentStore<EmailAttachment> _emailAttachmentStore;
        private readonly IEmailStore<EmailMessage> _emailStore;
        private readonly ILogger<EmailManager> _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly ISmtpService _smtpService;
        private readonly IBroker _broker;

        public EmailManager(
             IEmailAttachmentStore<EmailAttachment> emailAttachmentStore,
            IEmailStore<EmailMessage> emailStore,
            IOptions<SmtpSettings> options,
            ILogger<EmailManager> logger,
            ISmtpService smtpService,
            IBroker broker)
        {
            _emailAttachmentStore = emailAttachmentStore;
            _smtpSettings = options.Value;            
            _smtpService = smtpService;
            _emailStore = emailStore;
            _logger = logger;
            _broker = broker;
        }

        public async Task<ICommandResult<EmailMessage>> SaveAsync(MailMessage message)
        {

            var result = new CommandResult<EmailMessage>();

            // Ensure we've configured required email settings
            if (_smtpSettings?.DefaultFrom == null)
            {
                if (_logger.IsEnabled(LogLevel.Critical))
                {
                    _logger.LogCritical($"Error sending email \"{message.Subject}\". Outbound email settings must be configured via the admin dashboard before an email can be sent. No default 'From' address had been specified!");
                }
                return result.Failed($"Error sending email. Outbound email settings must be configured via the admin dashboard  before an email can be sent. No default 'From' address had been specified!");
            }

            // Use application email if no from is specified
            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }

            // Invoke EmailCreating subscriptions
            foreach (var handler in _broker.Pub<MailMessage>(this, "EmailCreating"))
            {
                message = await handler.Invoke(new Message<MailMessage>(message, this));
            }

            // Persist the message
            var email = await _emailStore.CreateAsync(message.ToEmailMessage());
            if (email != null)
            {
                // Invoke EmailCreated subscriptions
                foreach (var handler in _broker.Pub<MailMessage>(this,"EmailCreated"))
                {
                    message = await handler.Invoke(new Message<MailMessage>(message, this));
                }

                // Add email attachments
                foreach (var attachment in message.Attachments)
                {

                    var emailAttachhment = attachment.ToEmailAttachment();
                    emailAttachhment.EmailId = email.Id;

                    await _emailAttachmentStore.CreateAsync(emailAttachhment);

                }

                return result.Success(email);

            }

            return result.Failed($"An unknown error occurred whilst attempting to queue an email message");

        }

        public async Task<ICommandResult<MailMessage>> SendAsync(MailMessage message)
        {

            var result = new SmtpResult();

            // Ensure we've configured required email settings
            if (_smtpSettings?.DefaultFrom == null)
            {
                return result.Failed("Email settings must be configured before an email can be sent.");
            }

            // Use application email if no from is specified
            if (message.From == null)
            {
                message.From = new MailAddress(_smtpSettings.DefaultFrom);
            }

            // Invoke EmailSending subscriptions
            foreach (var handler in _broker.Pub<MailMessage>(this, "EmailSending"))
            {
                message = await handler.Invoke(new Message<MailMessage>(message, this));
            }
            
            // Attempt to send the email
            var sendResult = await _smtpService.SendAsync(message);
            if (sendResult.Succeeded)
            {
                // Invoke EmailSent subscriptions
                foreach (var handler in _broker.Pub<MailMessage>(this, "EmailSent"))
                {
                    message = await handler.Invoke(new Message<MailMessage>(message, this));
                }
                return result.Success(message);
            }

            return result.Failed(sendResult.Errors.ToArray());
            
        }

    }

}
