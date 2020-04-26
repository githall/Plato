using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;

namespace Plato.Tenants.Models
{

    /// <summary>
    /// Default tenant settings.
    /// </summary>
    public class TenantSettings : Serializable
    {

        public string ConnectionString { get; set; }

        public SmtpSettings SmtpSettings { get; set; }

    }

}
