using Microsoft.Extensions.Options;
using PlatoCore.Security.Abstractions.Encryption;

namespace PlatoCore.Security.Configuration
{

    public class PlatoKeyOptionsConfiguration : IConfigureOptions<PlatoKeyOptions>
    {

        private readonly IEncrypterKeyStore _keyStore;

        public PlatoKeyOptionsConfiguration(IEncrypterKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public void Configure(PlatoKeyOptions options)
        {
            var keyInfo = _keyStore.GetOrCreateKeysAsync()
                .GetAwaiter()
                .GetResult();
            options.Key = keyInfo.Key;
            options.Vector = keyInfo.Vector;
        }

    }

}
