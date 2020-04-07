using Plato.Files.Models;
using System.ComponentModel.DataAnnotations;

namespace Plato.Files.Sharing.ViewModels
{

    public class ShareFileViewModel
    {

        public File File { get; set; }

        [Required]
        public int FileId { get; set; }

        public string AttachmentEmail { get; set; }

        public string LinkEmail { get; set; }

    }

}
