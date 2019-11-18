using System.ComponentModel.DataAnnotations;

namespace Plato.GitHub.ViewModels
{

    public class GoogleSettingsViewModel
    {

        [StringLength(255), Display(Name = "client id")]
        public string ClientId { get; set; }

        [StringLength(255), Display(Name = "client secret")]
        public string ClientSecret { get; set; }

        [RegularExpression(@"\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]+[-A-Za-z0-9+&@#\/%=~_|]", ErrorMessage = "Invalid path")]
        public string CallbackPath { get; set; }

    }

}
