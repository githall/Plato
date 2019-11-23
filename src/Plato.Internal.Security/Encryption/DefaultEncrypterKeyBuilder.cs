using System;
using System.Security.Cryptography;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Encryption
{

    public class DefaultEncrypterKeyBuilder : IEncrypterKeyBuilder
    {

        public string Key => _key ?? throw new ArgumentNullException(nameof(_key));

        public string Vector => _vector ?? throw new ArgumentNullException(nameof(_vector));

        private readonly string _key;
        private readonly string _vector;

        public DefaultEncrypterKeyBuilder()
        {
            using (var aes = Aes.Create())
            {
                _key = Convert.ToBase64String(aes.Key);
                _vector = Convert.ToBase64String(aes.IV);
            }
        }

    }

}
