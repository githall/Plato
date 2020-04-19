using PlatoCore.Abstractions.SetUp;
using PlatoCore.Models.Shell;

namespace Plato.Tenants.Models
{
    public class TenantSetUpContext :SetUpContext
    {

        public TenantState State { get; set; }

    }
}
