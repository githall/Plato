using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class ContactFormViewModel
    {

        [Required, Display(Name = "name")]
        public string Name { get; set; }

        [Required, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string Email { get; set; }
        
        [Required, DataType(DataType.MultilineText), Display(Name = "message")]
        public string Message { get; set; }

    }

}
