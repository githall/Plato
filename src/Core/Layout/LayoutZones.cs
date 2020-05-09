
namespace PlatoCore.Layout
{
    public class LayoutZones
    {

        public const string HeaderZoneName = "header";
        public const string ToolsZoneName = "tools";
        public const string MetaZoneName = "meta";

        public const string ContentZoneName = "content";

        public const string ContentLeftZoneName = "content-left";
        public const string ContentRightZoneName = "content-right";

        public const string ContentFooterLeftZoneName = "content-footer-left";
        public const string ContentFooterRightZoneName = "content-footer-right";

        public const string ResizableHeaderLeft = "resizable-header-left";
        public const string ResizableHeaderRight = "resizable-header-right";
        public const string ResizableContent = "resizable-content";
        public const string ResizableFooterLeft = "resizable-footer-left";
        public const string ResizableFooterRight = "resizable-footer-right";

        public const string SideBarZoneName = "sidebar";
        public const string FooterZoneName = "footer";
        public const string ActionsZoneName = "actions";
        public const string AsidesZoneName = "asides";
        public const string AlertsZoneName = "alerts";

        public static string[] SupportedZones => new string[]
        {
            HeaderZoneName,
            ToolsZoneName,
            MetaZoneName,
            ContentZoneName,
            ContentLeftZoneName,
            ContentRightZoneName,
            ContentFooterLeftZoneName,
            ContentFooterRightZoneName,
            ResizableContent,
            ResizableFooterLeft,
            ResizableFooterRight,
            SideBarZoneName,
            FooterZoneName,
            ActionsZoneName,
            AsidesZoneName,
            AlertsZoneName
        };

    }

}
