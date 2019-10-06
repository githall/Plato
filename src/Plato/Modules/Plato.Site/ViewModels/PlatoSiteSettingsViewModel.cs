using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class PlatoSiteSettingsViewModel
    {

        [Required, StringLength(255), DataType(DataType.Url)]
        public string DemoUrl { get; set; }

    }

}
