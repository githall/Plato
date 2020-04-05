using System.IO;
using System.Net.Mail;

namespace PlatoCore.Emails.Abstractions.Extensions
{
    public static class EmailAttachmentExtensions
    {
        public static Attachment ToAttachment(this EmailAttachment attachment)
        {
            using var stream = new MemoryStream(attachment.ContentBlob);            
            return new Attachment(stream, attachment.Name, attachment.ContentType);
        }

    }

}
