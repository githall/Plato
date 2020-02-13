using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Layout.ModelBinding;

namespace PlatoCore.Layout.ViewProviders.Abstractions
{

    public abstract class ViewProviderBase<TModel> : IViewProvider<TModel> where TModel : class
    {

        public abstract Task<IViewProviderResult> BuildDisplayAsync(TModel viewModel, IViewProviderContext context);

        public abstract Task<IViewProviderResult> BuildIndexAsync(TModel viewModel, IViewProviderContext context);

        public abstract Task<IViewProviderResult> BuildEditAsync(TModel viewModel, IViewProviderContext context);

        public abstract Task<IViewProviderResult> BuildUpdateAsync(TModel viewModel, IViewProviderContext context);

        public virtual Task<bool> ValidateModelAsync(TModel model, IUpdateModel updater)
        {
            // We don't always need to implement ValidateModelAsync for certain views
            // For example views that don't perform any updates. We'll leave this method
            // virtual to allow us to override as and when needed within view provider implementations
            return Task.FromResult(true);
        }

        public virtual Task ComposeModelAsync(TModel model, IUpdateModel updater)
        {
            // We don't always need to implement ComposeModelAsync for certain views
            // For example views that don't perform any updates. We'll leave this method
            // virtual to allow us to override if needed within view provider implementations
            return Task.CompletedTask;
        }

        public IViewProviderResult Views(params IView[] views)
        {
            return new LayoutViewModel(new ViewProviderResult(views));
        }

        public IPositionedView View<TViewModel>(string viewName, Func<TViewModel, TViewModel> configure) where TViewModel : class
        {

            // Create proxy model 
            var proxy = ActivateInstanceOf<TViewModel>.Instance();

            // Configure model
            var model = configure((TViewModel) proxy);

            // Return a view we can optionally position
            return new PositionedView(viewName, model);

        }

    }

}