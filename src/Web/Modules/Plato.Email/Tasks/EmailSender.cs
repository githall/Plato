using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Email.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Emails.Abstractions.Extensions;
using PlatoCore.Tasks.Abstractions;

namespace Plato.Email.Tasks
{

    public class EmailSender : IBackgroundTaskProvider
    {

        public int IntervalInSeconds { get; private set; }

        private readonly IEmailAttachmentStore<EmailAttachment> _emailAttachmentStore;
        private readonly IEmailStore<EmailMessage> _emailStore;        
        private readonly ILogger<EmailSender> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IEmailManager _emailManager;
        private readonly IDbHelper _dbHelper;

        private readonly SmtpSettings _smtpSettings;

        public EmailSender(
             IEmailAttachmentStore<EmailAttachment> emailAttachmentStore,
            IEmailStore<EmailMessage> emailStore,
            IOptions<SmtpSettings> smtpSettings,
            ILogger<EmailSender> logger,
            IEmailManager emailManager,
            ICacheManager cacheManager,
            IDbHelper dbHelper)
        {

            _emailAttachmentStore = emailAttachmentStore;
            _smtpSettings = smtpSettings.Value;
            _cacheManager = cacheManager;
            _emailManager = emailManager;
            _emailStore = emailStore;
            _dbHelper = dbHelper;
            _logger = logger;

            // Set polling interval
            IntervalInSeconds = _smtpSettings.PollingInterval;

        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {

            // Polling is disabled
            if (!_smtpSettings.EnablePolling)
            {
                return;
            }

            // Get email to process
            var emails = await GetEmails();

            // We need emails to process
            if (emails == null)
            {
                return;
            }

            // Get all attachments for emails
            var attachments = await _emailAttachmentStore.QueryAsync()
                .Take(int.MaxValue, false)
                .Select<EmailAttachmentQueryParams>(q =>
                {
                    q.EmailId.IsIn(emails.Select(e => e.Id).ToArray());
                })
            .ToList();

            // Holds results to increment or delete emails
            var toDelete = new List<int>();
            var toIncrement = new List<int>();

            // Iterate emails attempting to send
            foreach (var email in emails)
            {

                // Build mail message
                var mailMessage = email.ToMailMessage();

                // Add attachments to the email
                foreach (var emailAttacment in attachments?.Data.Where(e => e.EmailId == email.Id))
                {
                    mailMessage.Attachments.Add(emailAttacment.ToAttachment());
                }            

                // Send the email
                var result = await _emailManager.SendAsync(mailMessage);

                // Success?
                if (result.Succeeded)
                {
                    // Queue for deletion
                    toDelete.Add(email.Id);
                }
                else
                {

                    // Log errors
                    if (_logger.IsEnabled(LogLevel.Critical))
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogCritical($"{error.Code} {error.Description}");
                        }
                    }

                    // Increment send attempts if we are below configured threshold
                    // Once we reach the threshold delete the email from the queue
                    if (email.SendAttempts < _smtpSettings.SendAttempts)
                    {
                        toIncrement.Add(email.Id);
                    }
                    else
                    {
                        toDelete.Add(email.Id);
                    }

                }

            }

            // Delete successfully sent 
            await ProcessToDelete(toDelete.ToArray());

            // Increment send attempts for failures
            await ProcessToIncrement(toIncrement.ToArray());

        }

        // -----------------

        async Task<IEnumerable<EmailMessage>> GetEmails()
        {
            var emails = await _emailStore.QueryAsync()
                .Take(_smtpSettings.BatchSize, false)
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
            return emails?.Data;
        }

        async Task ProcessToDelete(int[] ids)
        {

            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (ids.Length <= 0)
            {
                return;
            }
            
            // Execute query
            await _dbHelper.ExecuteScalarAsync<int>(
                "DELETE FROM {prefix}_Emails WHERE (Id IN ({ids}));",
                new Dictionary<string, string>()
                {
                    ["{ids}"] = ids.ToDelimitedString(',')
                });

            // Clear cache
            _cacheManager.CancelTokens(typeof(EmailStore));

        }

        async Task ProcessToIncrement(int[] ids)
        {

            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (ids.Length <= 0)
            {
                return;
            }

            // Execute query
            await _dbHelper.ExecuteScalarAsync<int>(
                "UPDATE {prefix}_Emails SET SendAttempts = (SendAttempts + 1) WHERE (Id IN ({ids}));",
                new Dictionary<string, string>()
                {
                    ["{ids}"] = ids.ToDelimitedString(',')
                });

            // Clear cache
            _cacheManager.CancelTokens(typeof(EmailStore));

        }
        
    }

}
