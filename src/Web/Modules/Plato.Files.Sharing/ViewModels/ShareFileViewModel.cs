using Plato.Files.Models;
using System.ComponentModel.DataAnnotations;

namespace Plato.Files.Sharing.ViewModels
{

    public class ShareFileViewModel
    {

        public File File { get; set; }

        [Required]
        public int FileId { get; set; }

        [EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string AttachmentEmail { get; set; }

        [EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string LinkEmail { get; set; }

    }

}
