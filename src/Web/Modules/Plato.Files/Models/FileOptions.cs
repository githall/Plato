namespace Plato.Files.Models
{
    public class FileOptions
    {

        public long MaxFileSize { get; set; }

        public long AvailableSpace { get; set; }

        public string[] AllowedExtensions { get; set; }

    }

}
