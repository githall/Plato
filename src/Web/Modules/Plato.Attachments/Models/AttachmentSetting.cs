using PlatoCore.Abstractions;
using System.Collections.Generic;

namespace Plato.Attachments.Models
{

    public class AttachmentSettings : Serializable
    {

        public IEnumerable<AttachmentSetting> Settings { get; set; }

    }

    public class AttachmentSetting : Serializable
    {

        public int RoleId { get; set; }

        public long AvailableSpace { get; set; }

        public string[] AllowedExtensions { get; set; }

    }

}
