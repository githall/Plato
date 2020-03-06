namespace Plato.Attachments.ViewModels
{

    public class UploadedFile
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string FriendlySize { get; set; }

        public bool IsImage { get; set; }

        public bool IsBinary { get; set; }

        public string Error { get; set; }

    }

}
