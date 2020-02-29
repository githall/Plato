using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace PlatoCore.Abstractions.Extensions
{
    public static class StreamExtensions
    {

        public static string Stringify(this Stream stream, Encoding encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (encoding == null)
                encoding = Encoding.UTF8;

            // Important: Ensure we reset the stream position
            stream.Position = 0;            
            using var reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd();
        }

        public static byte[] ToByteArray(this Stream stream)
        {

            // Important: Ensure we reset the stream position
            stream.Position = 0;

            // Create a byte array buffer matching the stream size
            var buffer = new byte[stream.Length];

            // Read the bytes from the steam
            for (var i = 0; i < stream.Length; i++)
            {
                i += stream.Read(
                    buffer,
                    i,
                    Convert.ToInt32(stream.Length) - i);
            }

            return buffer;

        }

        public static byte[] ToMD5(this Stream stream)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }

        public static string ToHex(this Stream stream, bool upperCase = false)
        {
            return stream.ToByteArray().ToHex(upperCase);
        }
    }

}
