using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Layout.Localizers
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
