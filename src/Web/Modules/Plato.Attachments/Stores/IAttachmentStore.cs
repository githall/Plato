using PlatoCore.Stores.Abstractions;

namespace Plato.Attachments.Stores
{
    public interface IAttachmentStore<TModel> : IStore<TModel> where TModel : class
    {
    }

}
