using System.ComponentModel.DataAnnotations;

namespace Plato.Tenants.ViewModels
{
    public class SmtpSettingsViewModel
    {

        [Required, StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "default from")]
        public string DefaultFrom { get; set; }

        [Required, StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "host")]
        public string Host { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "port")]
        public int Port { get; set; } = 25;

        public bool EnableSsl { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        public bool RequireCredentials { get; set; }

    }

}
