using System.Collections.Generic;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{

    public class LayoutOptions
    {

        public string ContentLeftCss { get; set; } = "layout-sidebar";

        public string ContentRightCss { get; set; } = "layout-asides";

        public LayoutViewModel Zones { get; set; }

    }

    public class LayoutViewModel : CombinedViewProviderResult
    {

        public IEnumerable<ILayoutZoneView> Header { get; set; }

        public IEnumerable<ILayoutZoneView> Tools { get; set; }

        public IEnumerable<ILayoutZoneView> Meta { get; set; }

        public IEnumerable<ILayoutZoneView> Content { get; set; }

        public IEnumerable<ILayoutZoneView> Actions { get; set; }

        public IEnumerable<ILayoutZoneView> ActionsRight { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableHeaderLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableHeaderRight { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableContent { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableFooterLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableFooterRight { get; set; }

        public IEnumerable<ILayoutZoneView> ContentLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ContentRight { get; set; }

        public IEnumerable<ILayoutZoneView> Footer { get; set; }

        public IEnumerable<ILayoutZoneView> FooterRight { get; set; }

        public IEnumerable<ILayoutZoneView> Alerts { get; set; }

        public LayoutViewModel(params IViewProviderResult[] results) : base(results)
        {
        }

        public LayoutViewModel BuildLayout()
        {
            return new LayoutComposition(base.GetResults()).Compose();
        }

    }

}
