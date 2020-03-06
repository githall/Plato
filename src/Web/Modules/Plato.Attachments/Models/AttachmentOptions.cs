namespace Plato.Attachments.Models
{
    public class AttachmentOptions
    {

        public string[] AllowedExtensions { get; set; }

        public long MaxFileSize { get; set; }

        public long AvailableSpace { get; set; }

    }

}
