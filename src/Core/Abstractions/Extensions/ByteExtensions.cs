using System.IO;
using System.Text;

namespace PlatoCore.Abstractions.Extensions
{

    public static class ByteExtensions
    {

        public static string Stringify(this byte[] bytes, Encoding encoding = null)
        {
            if (bytes == null) return null; 
            using var stream = new MemoryStream(bytes);
            return stream.Stringify(encoding);
        }

        public static byte[] ToMD5(this byte[] bytes)
        {
            if (bytes == null) return null;
            using var stream = new MemoryStream(bytes);
            return stream.ToMD5();
        }

        public static string ToHex(this byte[] bytes, bool upperCase = false)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            }
            return sb.ToString();
        }

    }

}
