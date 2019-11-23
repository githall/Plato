using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Encryption
{

    public class DefaultEncrypter : IEncrypter
    {

        private readonly PlatoKeyOptions _platoKeyOptions;

        private IKeyInfo _keyInfo;

        public DefaultEncrypter(IOptions<PlatoKeyOptions> optionsAccessor)
        {
            _platoKeyOptions = optionsAccessor.Value;
        }

        public string Encrypt(string plainText)
        {

            // We need input to encrypt
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            EnsureKeyInfo();            
            return Convert.ToBase64String(EncryptInternal(plainText));

        }

        public string Decrypt(string cipherText)
        {

            // We need input to decrypt
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            EnsureKeyInfo();            ;
            return DecryptInternal(Convert.FromBase64String(cipherText));

        }

        // -------------

        void EnsureKeyInfo()
        {
            if (_keyInfo == null)
            {

                if (string.IsNullOrEmpty(_platoKeyOptions.Key))
                {
                    throw new ArgumentNullException(nameof(_platoKeyOptions.Key));
                }

                if (string.IsNullOrEmpty(_platoKeyOptions.Vector))
                {
                    throw new ArgumentNullException(nameof(_platoKeyOptions.Vector));
                }

                _keyInfo = new KeyInfo(_platoKeyOptions.Key, _platoKeyOptions.Vector);

                if (_keyInfo.Key == null || _keyInfo.Key.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(_keyInfo.Key));
                }

                if (_keyInfo.Iv == null || _keyInfo.Iv.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(_keyInfo.Iv));
                }

            }

        }

        byte[] EncryptInternal(string plainText)
        {

            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {

                aesAlg.Key = _keyInfo.Key;
                aesAlg.IV = _keyInfo.Iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;

        }

        string DecryptInternal(byte[] cipherText)
        {

            string plaintext;
            using (var aesAlg = Aes.Create())
            {

                aesAlg.Key = _keyInfo.Key;
                aesAlg.IV = _keyInfo.Iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

    }

}
