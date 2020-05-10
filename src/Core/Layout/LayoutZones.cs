
namespace PlatoCore.Layout
{
    public class LayoutZones
    {

        // Alerts
        public const string Alerts = "alerts";

        // Header
        public const string Header = "header";
        public const string HeaderRight = "header-right";

        // Content        
        public const string ContentLeft = "content-left";
        public const string Content = "content";
        public const string ContentRight = "content-right";

        // Actions
        public const string Actions = "actions";
        public const string ActionsRight = "actions-right";

        // Footer
        public const string Footer = "footer";
        public const string FooterRight = "footer-right";

        // Resize
        public const string ResizeHeader = "resize-header";
        public const string ResizeHeaderRight = "resize-header-right";
        public const string ResizeContent = "resize-content";
        public const string ResizeActions = "resize-actions";
        public const string ResizeActionsRight = "resize-actions-right";

        public static string[] SupportedZones => new string[]
        {
            Alerts,
            Header,
            HeaderRight,     
            Content,
            ContentLeft,
            ContentRight,
            Actions,
            ActionsRight,     
            Footer,
            FooterRight,
            ResizeHeader,
            ResizeHeaderRight,
            ResizeContent,
            ResizeActions,
            ResizeActionsRight
        };

    }

}
