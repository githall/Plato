using Plato.Files.Models;

namespace Plato.Files.Sharing.ViewModels
{

    public class ShareFileViewModel
    {

        public File File { get; set; }

        public int FileId { get; set; }

        public string AttachmentEmail { get; set; }

        public string LinkEmail { get; set; }

    }

}
