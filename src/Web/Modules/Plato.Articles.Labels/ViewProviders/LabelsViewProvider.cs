using System;
using System.Threading.Tasks;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Data.Abstractions;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Articles.Labels.Models;
using Plato.Articles.Models;

namespace Plato.Articles.Labels.ViewProviders
{

    public class LabelViewProvider : ViewProviderBase<Label>
    {

        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;

        public LabelViewProvider(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade)
        {
            _featureFacade = featureFacade;
            _labelStore = labelStore;    
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Label label, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(LabelIndexViewModel<Label>)] as LabelIndexViewModel<Label>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(LabelIndexViewModel<Label>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<LabelIndexViewModel<Label>>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<LabelIndexViewModel<Label>>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<LabelIndexViewModel<Label>>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Label label, IViewProviderContext context)
        {

            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Article>)] as EntityIndexViewModel<Article>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Article>).ToString()} has not been registered on the HttpContext!");
            }

            var indexViewModel = new EntityIndexViewModel<Article>
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            // Get top 10 labels for feature
            var labels = await _labelStore.QueryAsync()
                .Take(10, false)
                .Select<LabelQueryParams>(async q =>
                {
                    q.FeatureId.Equals(await GetFeatureIdAsync());
                })
                .OrderBy("Entities", OrderBy.Desc)
                .ToList();

            return Views(
                View<Label>("Home.Display.Header", model => label).Zone("header").Order(1),
                View<Label>("Home.Display.Tools", model => label).Zone("tools").Order(1),
                View<EntityIndexViewModel<Article>>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<LabelsViewModel<Label>>("Article.Labels.Index.Sidebar", model =>
                {
                    model.SelectedLabelId = label?.Id ?? 0;
                    model.Labels = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Label model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Label model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        async Task<int> GetFeatureIdAsync()
        {
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
            if (feature != null)
            {
                return feature.Id;
            }

            throw new Exception($"Could not find required feature registration for Plato.Articles");
        }

    }

}
