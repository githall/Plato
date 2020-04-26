using System.ComponentModel.DataAnnotations;

namespace Plato.Tenants.ViewModels
{

    public class EditTenantSettingsViewModel
    {

        [Required, StringLength(500)]
        [Display(Name = "connection string")]
        public string ConnectionString { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "table prefix")]
        public string TablePrefix { get; set; }

        public SmtpSettingsViewModel SmtpSettings { get; set; } = new SmtpSettingsViewModel();

    }

}
