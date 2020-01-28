using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Layout.Localizers
{

    public class LocaleViewLocalizer :
        LocaleHtmlLocalizer,
        IViewLocalizer,
        IViewContextAware
    {
        public ViewContext ViewContext { get; private set; }

        public LocaleViewLocalizer(
            IOptions<LocaleOptions> localeOptions,
            ILocaleStore localeStore)
            : base(localeOptions, localeStore)
        {
        }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

    }

}
