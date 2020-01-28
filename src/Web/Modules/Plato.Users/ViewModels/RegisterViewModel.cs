using PlatoCore.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class RegisterViewModel
    {

        [Required, UserNameValidator, Display(Name = "username"), StringLength(100)]
        public string UserName { get; set; }

        [Required, EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string Email { get; set; }

        [Required, PasswordValidator, StringLength(100)]
        [DataType(DataType.Password), Display(Name = "password")]
        public string Password { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password), Compare("Password")]
        [Display(Name = "password confirmation")]
        public string ConfirmPassword { get; set; }

    }

}