using System;

namespace Plato.Internal.Security.Abstractions.Encryption
{

    public class KeyInfo : IKeyInfo
    {
        public byte[] Key { get; }

        public byte[] Iv { get; }

        public string KeyString => Convert.ToBase64String(Key);

        public string IVString => Convert.ToBase64String(Iv);

        protected KeyInfo()
        {
        }

        public KeyInfo(string key, string iv)
        {
            Key = Convert.FromBase64String(key);
            Iv = Convert.FromBase64String(iv);
        }

        public KeyInfo(byte[] key, byte[] iv)
        {
            Key = key;
            Iv = iv;
        }

    }

}
