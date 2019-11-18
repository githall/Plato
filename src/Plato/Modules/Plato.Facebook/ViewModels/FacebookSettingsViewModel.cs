using System.ComponentModel.DataAnnotations;

namespace Plato.Facebook.ViewModels
{
    public class FacebookSettingsViewModel
    {

        [StringLength(255), Display(Name = "app id")]
        public string AppId { get; set; }

        [StringLength(255), Display(Name = "app secret")]
        public string AppSecret { get; set; }

        [RegularExpression(@"\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]+[-A-Za-z0-9+&@#\/%=~_|]", ErrorMessage = "Invalid path")]
        public string CallbackPath { get; set; }

    }
}
