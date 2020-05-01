using PlatoCore.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Site.ViewModels
{
    public class SetUpViewModel
    {

        [Required, StringLength(255)]
        public string SessionId { get; set; }

        [Required, DataType(DataType.Text), Display(Name = "company name")]
        [StringLength(255, MinimumLength = 4)]
        public string CompanyName { get; set; }

    }

    public class SetUpConfirmationViewModel
    {

        [Required, StringLength(255)]
        public string SessionId { get; set; }

        [Required, UserNameValidator, Display(Name = "username"), StringLength(100)]
        public string UserName { get; set; }

        [Required, PasswordValidator, StringLength(100)]
        [DataType(DataType.Password), Display(Name = "password")]
        public string Password { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password), Compare("Password")]
        [Display(Name = "password confirmation")]
        public string ConfirmPassword { get; set; }

    }

}
