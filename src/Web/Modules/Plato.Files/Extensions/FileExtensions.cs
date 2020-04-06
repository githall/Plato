using Plato.Files.Models;
using System.Net.Mail;

namespace Plato.Files.Extensions
{
    public static class FileExtensions
    {

        public static Attachment ToAttachment(this File file)
        {
            return new Attachment(new System.IO.MemoryStream(file.ContentBlob), file.Name, file.ContentType);
        }

    }

}
