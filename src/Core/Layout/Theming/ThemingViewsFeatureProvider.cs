using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Primitives;
using PlatoCore.Layout.Razor;

namespace PlatoCore.Layout.Theming
{

    public class ThemingViewsFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {
            feature.ViewDescriptors.Add(new CompiledViewDescriptor()
            {
                ExpirationTokens = Array.Empty<IChangeToken>(),
                RelativePath = "/_ViewStart" + RazorViewEngine.ViewExtension,
                Item = new RazorViewCompiledItem(typeof(ThemeViewStart), @"mvc.1.0.view", "/_ViewStart")
            });
        }

    }

}
