using PlatoCore.Abstractions;
using System.Collections.Generic;

namespace Plato.Files.Models
{

    public class FileSettings : Serializable
    {

        public IEnumerable<FileSetting> Settings { get; set; }

    }

    public class FileSetting : Serializable
    {

        public int RoleId { get; set; }

        public long AvailableSpace { get; set; }

        public long MaxFileSize { get; set; }

        public string[] AllowedExtensions { get; set; }

    }

}
