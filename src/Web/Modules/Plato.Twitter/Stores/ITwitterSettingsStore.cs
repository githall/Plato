using PlatoCore.Stores.Abstractions;

namespace Plato.Twitter.Stores
{
    public interface ITwitterSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
