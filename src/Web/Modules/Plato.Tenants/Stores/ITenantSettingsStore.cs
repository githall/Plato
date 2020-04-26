using PlatoCore.Stores.Abstractions;

namespace Plato.Tenants.Stores
{
    public interface ITenantSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
