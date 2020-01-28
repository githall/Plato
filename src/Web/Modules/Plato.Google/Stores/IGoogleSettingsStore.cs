using PlatoCore.Stores.Abstractions;

namespace Plato.Google.Stores
{

    public interface IGoogleSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
