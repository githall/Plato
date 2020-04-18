using PlatoCore.Security.Attributes;
using PlatoCore.Shell.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Plato.Tenants.ViewModels
{
    public class EditTenantViewModel
    {

        [Required, Display(Name = "site name")]
        public string SiteName { get; set; }

        [Required, Display(Name = "connection string")]
        public string ConnectionString { get; set; }

        public bool ConnectionStringPreset { get; set; }

        [Required, Display(Name = "table prefix")]
        public string TablePrefix { get; set; }

        public bool TablePrefixPreset { get; set; }

        [Required, UserNameValidator, Display(Name = "username"), StringLength(255)]
        public string UserName { get; set; }

        [Required, EmailAddress, Display(Name = "email")]
        public string Email { get; set; }

        [Required, PasswordValidator, StringLength(100)]
        [DataType(DataType.Password), Display(Name = "password")]
        public string Password { get; set; }

        [Required, PasswordValidator, StringLength(100), Compare("Password")]
        [DataType(DataType.Password), Display(Name = "password confirmation")]
        public string PasswordConfirmation { get; set; }

        [ StringLength(100), Display(Name = "site prefix")]
        public string RequestedUrlPrefix { get; set; }

        [StringLength(100), Display(Name = "site host")]
        public string RequestedUrlHost { get; set; }

        public bool IsNewTenant { get; set; }

        public bool IsDefault => SiteName?.Equals(ShellHelper.DefaultShellName, System.StringComparison.OrdinalIgnoreCase) ?? false;      

    }

}
