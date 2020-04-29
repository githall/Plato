using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class GetStartedViewModel
    {

        [Required, DataType(DataType.EmailAddress), Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

        public bool EmailUpdates { get; set; }

    }
}
