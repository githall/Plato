using PlatoCore.Security.Abstractions;
using System.Collections.Generic;

namespace Plato.Attachments.Models
{
    public class DefaultAttachmentSettings
    {

        public const long AvailableSpace = 10485760;


        public static string[] AllowedExtensions = DefaultExtensions.AllowedExtensions;

    }

}
