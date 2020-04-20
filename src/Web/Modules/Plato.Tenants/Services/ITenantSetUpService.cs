using Plato.Tenants.Models;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace Plato.Tenants.Services
{
    public interface ITenantSetUpService
    {

        Task<ICommandResult<TenantSetUpContext>> InstallAsync(TenantSetUpContext context);

        Task<ICommandResult<TenantSetUpContext>> UpdateAsync(TenantSetUpContext context);

        Task<ICommandResultBase> UninstallAsync(string siteName);

    }

}
