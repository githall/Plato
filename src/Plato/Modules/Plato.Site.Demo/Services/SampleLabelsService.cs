using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Site.Demo.Services
{

    public class SampleLabelsService : ISampleLabelsService
    {

        public SampleLabelsService()
        {
        }

        public Task<ICommandResultBase> InstallAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResultBase> UninstallAsync()
        {
            throw new NotImplementedException();
        }
    }

}
