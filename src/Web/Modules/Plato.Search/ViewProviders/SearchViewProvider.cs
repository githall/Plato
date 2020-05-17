﻿using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Search.Models;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewProviders
{
    public class SearchViewProvider : ViewProviderBase<SearchResult>
    {
     
        public override Task<IViewProviderResult> BuildIndexAsync(SearchResult searchResult, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Entity>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Entity>>("Home.Index.Tools", model => viewModel).Zone("header-right"),                
                View<EntityIndexViewModel<Entity>>("Home.Index.Content", model => viewModel).Zone("content"),
                View<EntityIndexViewModel<Entity>>("Home.Index.Sidebar", model => viewModel).Zone("content-right").Order(3)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }


        public override Task<IViewProviderResult> BuildEditAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        

    }
}
