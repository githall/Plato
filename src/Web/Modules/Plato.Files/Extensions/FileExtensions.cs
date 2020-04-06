using Plato.Files.Models;
using System.Net.Mail;

namespace Plato.Files.Extensions
{
    public static class FileExtensions
    {

        public static Attachment ToAttachment(this File file)
        {
            using var stream = new System.IO.MemoryStream(file.ContentBlob);
            return new Attachment(stream, file.Name, file.ContentType);
        }
    }
}
