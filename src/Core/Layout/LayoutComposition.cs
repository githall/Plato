using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout
{
    public class LayoutComposition
    {

        private readonly IEnumerable<IViewProviderResult> _results;
        private readonly ConcurrentDictionary<string, IList<ILayoutZoneView>> _zonedViews =
            new ConcurrentDictionary<string, IList<ILayoutZoneView>>();

        public LayoutComposition(IEnumerable<IViewProviderResult> results)
        {
            _results = results;
        }

        public LayoutViewModel Compose()
        {

            // Create a zoned dictionary 
            ZoneResults();

            // Return composed model
            return new LayoutViewModel()
            {
                Header = GetPositionedViews(LayoutZones.Header),
                Tools = GetPositionedViews(LayoutZones.ToolsZoneName),
                Meta = GetPositionedViews(LayoutZones.MetaZoneName),

                Content = GetPositionedViews(LayoutZones.Content),

                Actions = GetPositionedViews(LayoutZones.Actions),
                ActionsRight = GetPositionedViews(LayoutZones.ActionsRight),

                ResizableHeaderLeft = GetPositionedViews(LayoutZones.ResizableHeaderLeft),
                ResizableHeaderRight = GetPositionedViews(LayoutZones.ResizableHeaderRight),
                ResizableContent = GetPositionedViews(LayoutZones.ResizableContent),
                ResizableFooterLeft = GetPositionedViews(LayoutZones.ResizableFooterLeft),
                ResizableFooterRight = GetPositionedViews(LayoutZones.ResizableFooterRight),

                ContentLeft = GetPositionedViews(LayoutZones.ContentLeft),                
                Footer = GetPositionedViews(LayoutZones.Footer),
                FooterRight = GetPositionedViews(LayoutZones.FooterRight),
                ContentRight = GetPositionedViews(LayoutZones.ContentRight),
                Alerts = GetPositionedViews(LayoutZones.AlertsZoneName),

            };

        }

        IEnumerable<ILayoutZoneView> GetPositionedViews(string[] zoneNames)
        {
            List<ILayoutZoneView> output = null;
            foreach (var zoneName in zoneNames)
            {
                var results = GetPositionedViews(zoneName);
                if (results != null)
                {
                    if (output == null)
                    {
                        output = new List<ILayoutZoneView>();
                    }
                    output.AddRange(results);
                }                
            }
            return output;
        }

        IEnumerable<ILayoutZoneView> GetPositionedViews(string zoneName)
        {

            // Returns an ordered list of all views within a zone. 
            if (_zonedViews.ContainsKey(zoneName))
            {
                return _zonedViews[zoneName].OrderBy(v => v.Position.Order);
            }

            return null;
        }

        void ZoneResults()
        {

            foreach (var result in _results)
            {

                if (result?.Views == null)
                {
                    continue;
                }

                foreach (var view in result.Views)
                {
                    // We can only zone views that implement IPositionedView
                    if (view is ILayoutZoneView positionedView)
                    {
                        _zonedViews.AddOrUpdate(positionedView.Position.Zone.ToLower(), new List<ILayoutZoneView>()
                        {
                            positionedView
                        }, (k, v) =>
                        {
                            v.Add(positionedView);
                            return v;
                        });
                    }
                }

            }

        }

    }

}
