using System.Collections.Generic;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{

    public class LayoutViewModel : CombinedViewProviderResult
    {

        public IEnumerable<ILayoutZoneView> LayoutBefore { get; set; }

        public IEnumerable<ILayoutZoneView> Alerts { get; set; }

        public IEnumerable<ILayoutZoneView> Header { get; set; }

        public IEnumerable<ILayoutZoneView> HeaderRight { get; set; }

        public IEnumerable<ILayoutZoneView> ContentLeft { get; set; }

        public IEnumerable<ILayoutZoneView> Tools { get; set; }

        public IEnumerable<ILayoutZoneView> ToolsRight { get; set; }

        public IEnumerable<ILayoutZoneView> Content { get; set; }

        public IEnumerable<ILayoutZoneView> Actions { get; set; }

        public IEnumerable<ILayoutZoneView> ActionsRight { get; set; }

        public IEnumerable<ILayoutZoneView> ContentRight { get; set; }

        public IEnumerable<ILayoutZoneView> Footer { get; set; }

        public IEnumerable<ILayoutZoneView> FooterRight { get; set; }

        public IEnumerable<ILayoutZoneView> LayoutAfter { get; set; }

        public IEnumerable<ILayoutZoneView> ResizeHeader { get; set; }

        public IEnumerable<ILayoutZoneView> ResizeHeaderRight { get; set; }

        public IEnumerable<ILayoutZoneView> ResizeContent { get; set; }

        public IEnumerable<ILayoutZoneView> ResizeActions { get; set; }

        public IEnumerable<ILayoutZoneView> ResizeActionsRight { get; set; }

        public LayoutViewModel(params IViewProviderResult[] results) : base(results)
        {
        }

        public LayoutViewModel BuildLayout()
        {
            return new LayoutComposition(base.GetResults()).Compose();
        }

    }

}
