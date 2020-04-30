using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class SignUpViewModel
    {

        [Required, DataType(DataType.EmailAddress), Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

        public bool EmailUpdates { get; set; }


   


    }


    public class SignUpConfirmationViewModel
    {

        [Required]
        public int Id { get; set; }


        public string Email { get; set; }

        [Required, DataType(DataType.Text), Display(Name = "confirmation code")]
        [StringLength(6, MinimumLength = 6)]
        public string SecurityToken { get; set; }

    }

}
