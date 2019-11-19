using System.ComponentModel.DataAnnotations;

namespace Plato.Google.ViewModels
{

    public class GoogleSettingsViewModel
    {

        // Auehtnicxation

        [StringLength(255), Display(Name = "client id")]
        public string ClientId { get; set; }

        [StringLength(255), Display(Name = "client secret")]
        public string ClientSecret { get; set; }

        [RegularExpression(@"\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]+[-A-Za-z0-9+&@#\/%=~_|]", ErrorMessage = "Invalid path")]
        public string CallbackPath { get; set; }
        
        // Analytics

        [StringLength(255), Display(Name = "tracking id")]
        public string TrackingId { get; set; }

    }

}
