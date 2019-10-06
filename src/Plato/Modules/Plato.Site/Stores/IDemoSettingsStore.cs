using Plato.Internal.Stores.Abstractions;

namespace Plato.Site.Stores
{
    public interface IPlatoSiteSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
