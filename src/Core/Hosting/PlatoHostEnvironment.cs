using Microsoft.Extensions.Hosting;
using PlatoCore.Hosting.Abstractions;

namespace PlatoCore.Hosting
{
    public class PlatoHostEnvironment : IPlatoHostEnvironment
    {

        private readonly IHostEnvironment _hostingEnvironment;

        protected PlatoHostEnvironment(
            IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string MapPath(string virtualPath)
        {
            return _hostingEnvironment.ContentRootPath + 
                virtualPath.Replace("~/", "");
        }

    }
}
