using System.Threading.Tasks;
using PlatoCore.Abstractions;

namespace Plato.Features.Updates.Services
{
    public interface IAutomaticFeatureMigrations
    {
        Task<ICommandResultBase> InitialMigrationsAsync();
    }

}
