using Plato.Internal.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plato.Site.Demo.Models;

namespace Plato.Site.Demo.Services
{

    public interface IDemoService
    {

        Task<ICommandResultBase> InstallEntitiesAsync(EntityDataDescriptor descriptor);

        Task<ICommandResultBase> InstallEntitiesAsync(IEnumerable<EntityDataDescriptor> descriptors);

        Task<ICommandResultBase> UninstallEntitiesAsync(EntityDataDescriptor descriptor);

        Task<ICommandResultBase> UninstallEntitiesAsync(IEnumerable<EntityDataDescriptor> descriptors);

        Task<ICommandResultBase> InstallUsersAsync();

    }

}
