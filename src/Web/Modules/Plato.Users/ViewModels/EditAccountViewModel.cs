using PlatoCore.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class EditAccountViewModel
    {

        public int Id { get; set; }

        [Required, UserNameValidator, StringLength(255), Display(Name = "username")]
        public string UserName { get; set; }
        
        [Required, EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

    }
}
