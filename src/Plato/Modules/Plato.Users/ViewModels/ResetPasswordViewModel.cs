using Plato.Internal.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{

    public class ResetPasswordViewModel
    {

        [Required, DataType(DataType.EmailAddress), Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password)]
        [Display(Name = "new password"), StringLength(100)]
        public string NewPassword { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password), Compare("NewPassword")]
        [Display(Name = "new password confirmation"), StringLength(100)]
        public string PasswordConfirmation { get; set; }

        public bool IsValidResetToken { get; set; }

        public string ResetToken { get; set; }

    }

}
