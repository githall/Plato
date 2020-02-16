using System;
using PlatoCore.Abstractions.Extensions;

namespace PlatoCore.Layout.Views.Abstractions
{

    public class LayoutZoneView : View, ILayoutZoneView
    {

        private string _zone;
        private int _order;

        public LayoutZoneView()
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }

        public LayoutZoneView(string viewName, object model) : base(viewName, model)
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }

        public ViewPosition Position => new ViewPosition(_zone, _order);

        public ILayoutZoneView Zone(string zone)
        {

            // We already expect a zone
            if (string.IsNullOrEmpty(zone))
            {
                throw new Exception(
                    $"No zone has been specified for the view {ViewName}.");
            }

            // Is the zone supported?            
            if (!LayoutZones.SupportedZones.Contains(zone, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    $"The zone name '{zone}' is not supported. Supported zones include {String.Join(", ", LayoutZones.SupportedZones)}. Please update the zone name within your view provider and try again.");
            }

            _zone = zone;
            return this;
        }

        public ILayoutZoneView Order(int order)
        {
            _order = order;
            return this;
        }

    }

}
