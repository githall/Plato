using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.ViewAdapters
{
    public interface IViewAdapterResult
    {

        IViewAdapterBuilder Builder { get; set; }

        IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations { get; set; }

        IList<string> ViewAlterations { get; set; }

        IList<Func<object, Task<object>>> ModelAlterations { get; set; }

    }

}
