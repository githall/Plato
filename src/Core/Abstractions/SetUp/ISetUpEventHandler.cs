using System;
using System.Threading.Tasks;

namespace PlatoCore.Abstractions.SetUp
{

    public interface ISetUpEventHandler
    {

        string ModuleId { get; }

        Task SetUp(
            ISetUpContext context,
            Action<string, string> reportError
        );

    }

}
