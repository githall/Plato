using Plato.Internal.Stores.Abstractions;

namespace Plato.Site.Demo.Stores
{
    public interface IDemoSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
