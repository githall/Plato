using PlatoCore.Models.Shell;
using System;
using System.Collections.Generic;
using System.Text;

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
