using PlatoCore.Models.Shell;
using System.Collections.Generic;

namespace Plato.Tenants.ViewModels
{
    public class TenantIndexViewModel
    {

        public TenantIndexOptions Options { get; set; }

        public IEnumerable<ShellSettings> Results { get; set; }

    }

    public class TenantIndexOptions
    {

    }

}
