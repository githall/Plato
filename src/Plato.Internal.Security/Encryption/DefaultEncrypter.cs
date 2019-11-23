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

        public string Encrypt(string input)
        {

            // We need input to encrypt
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            EnsureKeyInfo();
            var enc = EncryptInternal(input, _keyInfo.Key, _keyInfo.Iv);
            return Convert.ToBase64String(enc);

        }

        public string Decrypt(string input)
        {

            // We need input to decrypt
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            EnsureKeyInfo();
            var cipherBytes = Convert.FromBase64String(input);
            return DecryptInternal(cipherBytes, _keyInfo.Key, _keyInfo.Iv);

        }

        // -------------

        void EnsureKeyInfo()
        {
            if (_keyInfo == null)
            {

                if (string.IsNullOrEmpty(_platoKeyOptions.Key) || string.IsNullOrEmpty(_platoKeyOptions.Vector))
                {
                    throw new Exception("Plato keys have not been configured correctly!");
                }

                _keyInfo = new KeyInfo(_platoKeyOptions.Key, _platoKeyOptions.Vector);

            }
            
        }

        static byte[] EncryptInternal(string plainText, byte[] key, byte[] iv)
        {

            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

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

        static string DecryptInternal(byte[] cipherText, byte[] key, byte[] iv)
        {

            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }                

            string plaintext;
            using (var aesAlg = Aes.Create())
            {

                aesAlg.Key = key;
                aesAlg.IV = iv;

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
