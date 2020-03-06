namespace Plato.Attachments.Models
{
    public class DefaultAttachmentSettings
    {

        public const long MaxFilesize = 2097152; // 2mb


        public const long AvailableSpace = 10485760; // 10mb


        public static string[] AllowedExtensions = DefaultExtensions.AllowedExtensions;

    }

}
