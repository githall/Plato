using Plato.Internal.Stores.Abstractions;

namespace Plato.GitHub.Stores
{

    public interface IGitHubSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
