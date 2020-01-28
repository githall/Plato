using System.Collections.Generic;

namespace PlatoCore.Localization.Abstractions.Models
{

    public class ComposedLocaleDescriptor
    {

        public LocaleDescriptor Descriptor { get; set; }

        public IEnumerable<ComposedLocaleResource> Resources { get; set; }

    }

}
