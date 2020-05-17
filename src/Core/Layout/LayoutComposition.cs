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

                // Layout
                LayoutBefore = GetPositionedViews(LayoutZones.LayoutBefore),
                LayoutAfter = GetPositionedViews(LayoutZones.LayoutAfter),

                // Header
                Alerts = GetPositionedViews(LayoutZones.Alerts),
                Header = GetPositionedViews(LayoutZones.Header),
                HeaderRight = GetPositionedViews(LayoutZones.HeaderRight),

                // Content tools
                Tools = GetPositionedViews(LayoutZones.Tools),
                ToolsRight = GetPositionedViews(LayoutZones.ToolsRight),

                // Content
                ContentLeft = GetPositionedViews(LayoutZones.ContentLeft),
                Content = GetPositionedViews(LayoutZones.Content),
                ContentRight = GetPositionedViews(LayoutZones.ContentRight),

                // Content actions
                Actions = GetPositionedViews(LayoutZones.Actions),
                ActionsRight = GetPositionedViews(LayoutZones.ActionsRight),

                // Footer
                Footer = GetPositionedViews(LayoutZones.Footer),
                FooterRight = GetPositionedViews(LayoutZones.FooterRight),

                // Resize
                ResizeHeader = GetPositionedViews(LayoutZones.ResizeHeader),
                ResizeHeaderRight = GetPositionedViews(LayoutZones.ResizeHeaderRight),
                ResizeContent = GetPositionedViews(LayoutZones.ResizeContent),
                ResizeActions = GetPositionedViews(LayoutZones.ResizeActions),
                ResizeActionsRight = GetPositionedViews(LayoutZones.ResizeActionsRight)

            };

        }

        private IEnumerable<ILayoutZoneView> GetPositionedViews(string[] zoneNames)
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

        private IEnumerable<ILayoutZoneView> GetPositionedViews(string zoneName)
        {

            // Returns an ordered list of all views within a zone. 
            if (_zonedViews.ContainsKey(zoneName))
            {
                return _zonedViews[zoneName].OrderBy(v => v.Position.Order);
            }

            return null;
        }

        private void ZoneResults()
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
