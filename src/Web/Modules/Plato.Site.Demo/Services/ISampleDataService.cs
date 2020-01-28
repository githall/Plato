using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Site.Demo.Services
{

    public interface ISampleDataService
    {

        Task<ICommandResultBase> InstallAsync();

        Task<ICommandResultBase> UninstallAsync();

    }

}
