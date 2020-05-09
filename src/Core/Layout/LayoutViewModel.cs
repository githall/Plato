using System.Collections.Generic;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{

    public class LayoutOptions
    {

        public string BreadCrumbCss { get; set; } = "layout-breadcrumb mt-3";

        public string HeaderCss { get; set; } = "layout-header layout-header-sticky";

        public string ContentLeftCss { get; set; } = "col-lg-3 layout-sidebar";

        public string ContentRightCss { get; set; } = "col-lg-3 layout-sidebar";

        public LayoutViewModel Zones { get; set; }

    }

    public class LayoutViewModel : CombinedViewProviderResult
    {

        public IEnumerable<ILayoutZoneView> Header { get; set; }

        public IEnumerable<ILayoutZoneView> Tools { get; set; }

        public IEnumerable<ILayoutZoneView> Meta { get; set; }

        public IEnumerable<ILayoutZoneView> Content { get; set; }

        public IEnumerable<ILayoutZoneView> ContentFooterLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ContentFooterRight { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableHeaderLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableHeaderRight { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableContent { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableFooterLeft { get; set; }

        public IEnumerable<ILayoutZoneView> ResizableFooterRight { get; set; }

        public IEnumerable<ILayoutZoneView> SideBar { get; set; }

        public IEnumerable<ILayoutZoneView> Asides { get; set; }

        public IEnumerable<ILayoutZoneView> Footer { get; set; }

        public IEnumerable<ILayoutZoneView> Actions { get; set; }
 
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
