using Microsoft.Extensions.Primitives;

namespace PlatoCore.Cache.Abstractions
{

    public interface ICacheDependency
    {
        IChangeToken GetToken(string key);

        void CancelToken(string key);

    }

}
