using PlatoCore.Abstractions;

namespace Plato.Site.Models
{
    public class PlatoSiteSettings : Serializable
    {

        public string HostUrl { get; set; }

        public string DemoUrl { get; set; }

        public string PlatoDesktopUrl { get; set; }

    }

}
