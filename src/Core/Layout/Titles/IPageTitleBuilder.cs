using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;

namespace PlatoCore.Layout.Titles
{
    public interface IPageTitleBuilder
    {

        IPageTitleBuilder Clear();

        IPageTitleBuilder AddSegment(LocalizedString segment, int order = 0);
        
        IHtmlContent GenerateTitle(IHtmlContent separator);

    }

}
