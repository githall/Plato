﻿using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Models;
using Plato.Discuss.Tags.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class TagViewProvider : ViewProviderBase<Tag>
    {

        private readonly IFeatureFacade _featureFacade;
        private readonly ITagStore<Tag> _tagStore;

        public TagViewProvider(
            ITagStore<Tag> tagStore,
            IFeatureFacade featureFacade)
        {
            _tagStore = tagStore;
            _featureFacade = featureFacade;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Tag tag, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(TagIndexViewModel<Tag>)] as TagIndexViewModel<Tag>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TagIndexViewModel<Tag>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<TagIndexViewModel<Tag>>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<TagIndexViewModel<Tag>>("Home.Index.Tools", model => viewModel).Zone("header-right").Order(1),
                View<TagIndexViewModel<Tag>>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Tag tag, IViewProviderContext context)
        {

            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }

            var indexViewModel = new TagDisplayViewModel
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get all tags for feature
            var tags = await _tagStore.QueryAsync()
                .Take(20, false)
                .Select<TagQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            // Build view
            return Views(
                View<TagBase>("Home.Display.Header", model => tag).Zone("header").Order(1),
                View<TagBase>("Home.Display.Tools", model => tag).Zone("header-right").Order(1),
                View<TagDisplayViewModel>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<TagsViewModel<Tag>>("Topic.Tags.Index.Sidebar", model =>
                {
                    model.SelectedTagId = tag?.Id ?? 0;
                    model.Tags = tags?.Data;
                    return model;
                }).Zone("content-right").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Tag tag, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Tag tag, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        #endregion

    }

}
