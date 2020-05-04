using System.ComponentModel.DataAnnotations;

namespace Plato.Tenants.SignUp.ViewModels
{
    public class PlatoSiteSettingsViewModel
    {

        [StringLength(255)]
        public string HostUrl { get; set; }

        [StringLength(255)]
        public string DemoUrl { get; set; }

        [StringLength(255)]
        public string PlatoDesktopUrl { get; set; }

    }

}
