using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Encryption
{
    public class DefaultEncrypter : IEncrypter
    {

        private readonly IAesEncrypter _encrypter;

        public DefaultEncrypter(IAesEncrypter encrypter)
        {
            _encrypter = encrypter;
        }

        public string Encrypt(string input)
        {
            return _encrypter.Encrypt(input);
        }

        public string Decrypt(string input)
        {
            return _encrypter.Decrypt(input);
        }

    }

}
