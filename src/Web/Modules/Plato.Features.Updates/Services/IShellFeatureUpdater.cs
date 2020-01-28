using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace Plato.Features.Updates.Services
{

    public interface IShellFeatureUpdater
    {
        Task<ICommandResultBase> UpdateAsync(string featureId);
    }

}
