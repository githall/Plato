using System.ComponentModel.DataAnnotations;

namespace Plato.Google.ViewModels
{
    public class GoogleSettingsViewModel
    {

        [StringLength(255), Display(Name = "client id")]
        public string ClientId { get; set; }

        [StringLength(255), Display(Name = "client secret")]
        public string ClientSecret { get; set; }

    }
}
