using System;
using System.Net.Mail;
using PlatoCore.Abstractions.Extensions;

namespace PlatoCore.Emails.Abstractions.Extensions
{
    public static class AttachmentExtensions
    {

        public static EmailAttachment ToEmailAttachment(this Attachment attachment)
        {

            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            if (string.IsNullOrEmpty(attachment.Name))
            {
                throw new ArgumentNullException(nameof(attachment.Name));
            }

            if (attachment.ContentType == null)
            {
                throw new ArgumentNullException(nameof(attachment.ContentType));
            }

            if (attachment.ContentStream == null)
            {
                throw new ArgumentNullException(nameof(attachment.ContentStream));
            }

            var bytes = attachment.ContentStream.ToByteArray();

            return new EmailAttachment()
            {
                Name = attachment.Name,
                ContentBlob = bytes,
                ContentLength = bytes.Length,
                ContentType = attachment.ContentType.ToString()
            };

        }

    }

}
