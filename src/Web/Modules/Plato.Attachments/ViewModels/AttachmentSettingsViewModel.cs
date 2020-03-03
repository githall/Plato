namespace Plato.Attachments.ViewModels
{
    public class AttachmentSettingsViewModel
    {

        public string IconPrefix { get; } = "fiv-sqo fiv-size-md fiv-icon-";

        public string[] DefaultExtensions { get; set; }

        public string[] AllowedExtensions { get; set; }

        public string ExtensionHtmlName { get; set; }

    }

}
