using System.Collections.Generic;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{
    public class LayoutViewModel : CombinedViewProviderResult
    {

        public IEnumerable<ILayoutZoneView> Header { get; set; }

        public IEnumerable<ILayoutZoneView> Tools { get; set; }

        public IEnumerable<ILayoutZoneView> Meta { get; set; }

        public IEnumerable<ILayoutZoneView> Content { get; set; }

        public IEnumerable<ILayoutZoneView> SideBar { get; set; }

        public IEnumerable<ILayoutZoneView> Footer { get; set; }

        public IEnumerable<ILayoutZoneView> Actions { get; set; }

        public IEnumerable<ILayoutZoneView> Asides { get; set; }

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
