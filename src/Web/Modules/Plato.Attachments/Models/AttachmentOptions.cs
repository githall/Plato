using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Attachments.Models
{
    public class AttachmentOptions
    {

        public string[] AllowedExtensions { get; set; }

        public long AvailableSpace { get; set; }

    }
}
