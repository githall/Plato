using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Layout.ViewAdapters;

namespace PlatoCore.Layout.Views.Abstractions
{
    public class ViewDisplayContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public IViewDisplayHelper DisplayAsync { get; set; }

        public ViewContext ViewContext { get; set; }

        public IEnumerable<IViewAdapterResult> ViewAdapterResults { get; set; }

        public object Value { get; set; }
        
        public ViewDescriptor ViewDescriptor { get; set; }

    }

}
