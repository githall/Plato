using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Tags.Services
{
    public interface ITagService<TModel> where TModel : class, ITag
    {
        Task<IPagedResults<TModel>> GetResultsAsync(TagIndexOptions options, PagerOptions pager);
    }


}
