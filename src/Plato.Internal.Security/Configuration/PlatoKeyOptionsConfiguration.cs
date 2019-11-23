using Microsoft.Extensions.Options;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Configuration
{

    class PlatoKeyOptionsConfiguration : IConfigureOptions<PlatoKeyOptions>
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
