using System.Threading.Tasks;
using Plato.Email.Stores;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Email.Services
{

    public class EmailSettingsManager : IEmailSettingsManager
    {

        private readonly IEmailSettingsStore<EmailSettings> _emailSettingsStore;
        private readonly IBroker _broker;

        public EmailSettingsManager(
            IEmailSettingsStore<EmailSettings> emailSettingsStore,
            IBroker broker)
        {
            _emailSettingsStore = emailSettingsStore;
            _broker = broker;
        }

        public async Task<EmailSettings> SaveAsync(EmailSettings settings)
        {

            // Invoke EmailSettingsUpdating subscriptions
            foreach (var handler in _broker.Pub<EmailSettings>(this, "EmailSettingsUpdating"))
            {
                settings = await handler.Invoke(new Message<EmailSettings>(settings, this));
            }

            var result = await _emailSettingsStore.SaveAsync(settings);
            if (result != null)
            {

                // Invoke EmailSettingsUpdated subscriptions
                foreach (var handler in _broker.Pub<EmailSettings>(this, "EmailSettingsUpdated"))
                {
                    result = await handler.Invoke(new Message<EmailSettings>(result, this));
                }

            }

            return result;

        }

    }

}
