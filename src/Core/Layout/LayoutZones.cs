
namespace PlatoCore.Layout
{
    public class LayoutZones
    {

        public const string Header = "header";
        public const string ToolsZoneName = "tools";
        public const string MetaZoneName = "meta";
        public const string AlertsZoneName = "alerts";

        // Content
        public const string Content = "content";
        public const string ContentLeft = "content-left";
        public const string ContentRight = "content-right";

        // Actions
        public const string Actions = "actions";
        public const string ActionsRight = "actions-right";

        // Footer
        public const string Footer = "footer";
        public const string FooterRight = "footer-right";

        // Resizable 
        public const string ResizableHeaderLeft = "resizable-header-left";
        public const string ResizableHeaderRight = "resizable-header-right";
        public const string ResizableContent = "resizable-content";
        public const string ResizableFooterLeft = "resizable-footer-left";
        public const string ResizableFooterRight = "resizable-footer-right";

        public static string[] SupportedZones => new string[]
        {
            Header,
            ToolsZoneName,
            MetaZoneName,
            Content,
            ContentLeft,
            ContentRight,
            Actions,
            ActionsRight,
            ResizableContent,
            ResizableFooterLeft,
            ResizableFooterRight,
            Footer,
            FooterRight,          
            AlertsZoneName
        };

    }

}
