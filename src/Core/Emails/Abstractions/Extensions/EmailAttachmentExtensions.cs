using System.IO;
using System.Net.Mail;

namespace PlatoCore.Emails.Abstractions.Extensions
{
    public static class EmailAttachmentExtensions
    {
        public static Attachment ToAttachment(this EmailAttachment attachment)
        {     
            return new Attachment(new MemoryStream(attachment.ContentBlob), attachment.Name, attachment.ContentType);
        }

    }

}
