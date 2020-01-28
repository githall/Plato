using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using PlatoCore.Security.Abstractions.Encryption;

namespace PlatoCore.Security.Encryption
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

            byte[] encrypted = null;
            using (var aes = Aes.Create())
            {

                aes.Key = _keyInfo.Key;
                aes.IV = _keyInfo.Iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }

            return encrypted;

        }

        string DecryptInternal(byte[] cipherText)
        {

            string plainText = null;
            using (var aes = Aes.Create())
            {

                aes.Key = _keyInfo.Key;
                aes.IV = _keyInfo.Iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(cipherText))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            plainText = sr.ReadToEnd();
                        }
                    }
                }

            }

            return plainText;

        }

    }

}
