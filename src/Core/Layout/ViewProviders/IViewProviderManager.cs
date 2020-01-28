using System.Threading.Tasks;
using PlatoCore.Layout.ModelBinding;

namespace PlatoCore.Layout.ViewProviders
{
    public interface IViewProviderManager<TModel> where TModel : class
    {

        Task<IViewProviderResult> ProvideDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideIndexAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideEditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideUpdateAsync(TModel model, IUpdateModel updater);

        Task<bool> IsModelStateValidAsync(TModel model, IUpdateModel updater);
        
        Task<TModel> ComposeModelAsync(TModel model, IUpdateModel updater);
     
    }

}
