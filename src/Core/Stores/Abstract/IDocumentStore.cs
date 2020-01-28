using System.Threading.Tasks;
using PlatoCore.Models;

namespace PlatoCore.Stores.Abstract
{
    public interface IDocumentStore
    {

        Task<TModel> GetAsync<TModel>() where TModel : class;

        Task<TModel> SaveAsync<TModel>(IDocument document) where TModel : class;

        Task<bool> DeleteAsync<TModel>() where TModel : class;

    }
}
