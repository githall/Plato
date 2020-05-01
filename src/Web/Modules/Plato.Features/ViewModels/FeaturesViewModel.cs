﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Models.Features;

namespace Plato.Features.ViewModels
{
    
    public class FeaturesIndexViewModel
    {

        public FeatureIndexOptions Options { get; set; } = new FeatureIndexOptions();

        public IEnumerable<SelectListItem> AvailableCategories { get; set; }

        public IEnumerable<IShellFeature> Features { get; set; }

    }

    public class FeatureIndexOptions
    {

        public string[] ModuleIds { get; set; }

        public string Category { get; set; }

        public bool HideEnabled { get; set; }

    }
    
}
