using Microsoft.Extensions.Hosting;

namespace PlatoCore.Hosting
{
    public class PlatoWebHostEnvironment : PlatoHostEnvironment
    {
        public PlatoWebHostEnvironment(IHostEnvironment hostingEnvironment) 
            : base(hostingEnvironment)
        {

        }

    }

}
