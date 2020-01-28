using PlatoCore.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Site.Demo.ViewModels
{
    public class DemoSettingsViewModel
    {

        public bool DemoEnabled { get; set; }

        [Required, UserNameValidator, Display(Name = "username"), StringLength(100)]
        public string AdminUserName { get; set; }

        [Required, PasswordValidator, DataType(DataType.Password)]
        [Display(Name = "password"), StringLength(100)]
        public string AdminPassword { get; set; }

    }

}
