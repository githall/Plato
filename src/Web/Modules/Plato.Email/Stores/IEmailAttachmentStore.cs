using PlatoCore.Stores.Abstractions;
using System.Threading.Tasks;

namespace Plato.Email.Stores
{
    public interface IEmailAttachmentStore<TModel> : IStore<TModel> where TModel : class
    {  
    }

}
