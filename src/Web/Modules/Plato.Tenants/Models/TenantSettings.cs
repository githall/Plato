using PlatoCore.Abstractions;

namespace Plato.Tenants.Models
{

    /// <summary>
    /// Default tenant settings.
    /// </summary>
    public class TenantSettings : Serializable
    {

        public string ConnectionString { get; set; }

    }

}
