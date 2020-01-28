using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace PlatoCore.Hosting.Web
{
    public class WebHostEnvironment : PlatoHostEnvironment
    {

        public WebHostEnvironment(
            IHostEnvironment hostingEnvironment) : 
            base(hostingEnvironment)
        {
            T = null;
        }

        public IStringLocalizer T { get; set; }

    }

}
