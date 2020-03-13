using PlatoCore.Stores.Abstractions;

namespace Plato.Files.Stores
{
    public interface IFileSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
