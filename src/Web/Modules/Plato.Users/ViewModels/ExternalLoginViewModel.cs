using PlatoCore.Security.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{

    public class ExternalLoginViewModel : IValidatableObject
    {
        public bool IsExistingUser { get; set; }

        [Required, UserNameValidator, Display(Name = "username"), StringLength(100)]
        public string UserName { get; set; }

        [Required, EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsExistingUser)
            {
                if (Password != ConfirmPassword)
                {
                    yield return new ValidationResult("Confirm Password do not match", new[] { "ConfirmPassword" });
                }

                if (Password != null && (Password.Length < 6 || Password.Length > 100))
                {
                    yield return new ValidationResult(string.Format("Password must be between {0} and {1} characters", 6, 100), new[] { "Password" });
                }
            }
        }

    }

}
