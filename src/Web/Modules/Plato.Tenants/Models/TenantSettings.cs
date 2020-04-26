using PlatoCore.Abstractions;
using PlatoCore.Emails.Abstractions;

namespace Plato.Tenants.Models
{

    /// <summary>
    /// Default tenant settings.
    /// </summary>
    public class TenantSettings : Serializable
    {

        public string ConnectionString { get; set; } = "server=localhost;trusted_connection=true;database=plato";

        public string TablePrefix { get; set; } = "plato";

        public SmtpSettings SmtpSettings { get; set; } = new SmtpSettings();

    }

}
