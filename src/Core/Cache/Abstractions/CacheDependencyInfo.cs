using System.Threading;
using Microsoft.Extensions.Primitives;

namespace PlatoCore.Cache.Abstractions
{

    public class CacheDependencyInfo
    {

        public IChangeToken ChangeToken { get; set; }

        public CancellationTokenSource CancellationToken { get; set; }

    }

}
