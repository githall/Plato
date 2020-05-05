using Plato.Tenants.Models;
using PlatoCore.Abstractions;
using PlatoCore.Models.Shell;
using System.Threading.Tasks;

namespace Plato.Tenants.Services
{
    public interface ITenantSetUpService
    {

        Task<ICommandResult<TenantSetUpContext>> InstallAsync(TenantSetUpContext context);

        Task<ICommandResult<TenantSetUpContext>> UpdateAsync(TenantSetUpContext context);

        Task<ICommandResultBase> UninstallAsync(IShellSettings shellSettings);

    }

}
