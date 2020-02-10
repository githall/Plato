using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.ViewAdapters.Abstractions
{
    public interface IViewAdapterBuilder
    {

        string ViewName { get; }

        IViewAdapterBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);

        IViewAdapterBuilder AdaptView(string viewName);

        IViewAdapterBuilder AdaptView(string[] viewNames);

        IViewAdapterBuilder AdaptModel<TModel>(Func<TModel, Task<object>> alteration) where TModel : class;

    }

}
