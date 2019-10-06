using Plato.Internal.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class EditPasswordViewModel
    {

        [Required]
        public string Id { get; set; }

        [Required]
        public string ResetToken { get; set; }

        [Required]
        public string Email { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password)]
        [Display(Name = "new password"), StringLength(100)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), StringLength(100)]
        [Display(Name = "new password confirmation")]
        public string PasswordConfirmation { get; set; }

        public bool SendEmail { get; set; }


    }
}
