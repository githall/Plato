namespace Plato.Attachments.Models
{
    public class AttachmentOptions
    {

        public long MaxFileSize { get; set; }

        public long AvailableSpace { get; set; }

        public string[] AllowedExtensions { get; set; }

    }

}
