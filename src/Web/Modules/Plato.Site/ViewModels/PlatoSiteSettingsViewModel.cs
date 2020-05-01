using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class PlatoSiteSettingsViewModel
    {

        [StringLength(255), DataType(DataType.Url)]
        public string HostUrl { get; set; }

        [StringLength(255), DataType(DataType.Url)]
        public string DemoUrl { get; set; }

        [StringLength(255), DataType(DataType.Url)]
        public string PlatoDesktopUrl { get; set; }

    }

}
