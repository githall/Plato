using Plato.Internal.Abstractions;

namespace Plato.Site.Demo.Models
{
    public class DemoSettings : Serializable
    {

        public bool DemoEnabled { get; set; }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

    }

}
