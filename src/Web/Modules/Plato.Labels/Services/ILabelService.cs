using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation;
using PlatoCore.Navigation.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Labels.Services
{
    public interface ILabelService<TModel> where TModel : class, ILabel
    {
        Task<IPagedResults<TModel>> GetResultsAsync(LabelIndexOptions options, PagerOptions pager);
    }

}
