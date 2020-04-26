using System.ComponentModel.DataAnnotations;

namespace Plato.Tenants.ViewModels
{

    public class EditTenantSettingsViewModel
    {

        [Required, StringLength(500)]
        [Display(Name = "connection string")]
        public string ConnectionString { get; set; }

    }

}
