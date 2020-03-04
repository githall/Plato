using PlatoCore.Stores.Abstractions;

namespace Plato.Attachments.Stores
{
    public interface IAttachmentSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
