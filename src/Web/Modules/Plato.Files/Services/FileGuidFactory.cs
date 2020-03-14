using System;
using System.Text;
using PlatoCore.Text.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Files.Services
{

    public class FileGuidFactory : IFileGuidFactory
    {

        private readonly IKeyGenerator _keyGenerator;

        public FileGuidFactory(IKeyGenerator keyGenerator)
        {
            _keyGenerator = keyGenerator;
        }

        public string NewGuid(string uniqueKey)
        {

            // Create a 256 bit unique ASCII string
            var key = _keyGenerator.GenerateKey(o =>
            {
                o.MaxLength = 32;
                o.UniqueIdentifier = uniqueKey;
            });

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Transform to 256 bit / 32 character hexadecimal string
           return key.ToStream(Encoding.ASCII).ToMD5().ToHex();

        }

    }

}
