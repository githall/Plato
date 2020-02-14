using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace PlatoCore.FileSystem
{
    public class HostedFileSystem : PlatoFileSystem
    {
        public HostedFileSystem(
            IHostEnvironment hostingEnvironment,
            ILogger<HostedFileSystem> logger) :
            base(
                hostingEnvironment.ContentRootPath,
                hostingEnvironment.ContentRootFileProvider,
                logger)
        { 
        }
    }

}
