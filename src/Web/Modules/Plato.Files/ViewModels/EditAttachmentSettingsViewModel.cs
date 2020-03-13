using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Models.Roles;

namespace Plato.Files.ViewModels
{
    public class EditAttachmentSettingsViewModel
    {

        public string IconPrefix { get; } = "fiv-sqo fiv-size-md fiv-icon-";

        public string[] DefaultExtensions { get; set; }

        public string[] AllowedExtensions { get; set; }

        public string ExtensionHtmlName { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        [Required]
        public long MaxFileSize { get; set; }

        [Required]
        public long AvailableSpace { get; set; }

        public IEnumerable<SelectListItem> AvailableSpaces { get; set; }

    }

}
