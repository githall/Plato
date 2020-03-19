namespace Plato.Files.Models
{
    public class FileOptions
    {

        public long MaxFileSize { get; set; } = DefaultFileOptions.MaxFileSize;

        public long AvailableSpace { get; set; } = DefaultFileOptions.AvailableSpace;

        public string[] AllowedExtensions { get; set; } = DefaultFileOptions.AllowedExtensions;

    }

}
