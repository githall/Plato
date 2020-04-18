﻿using System.Threading.Tasks;
using Plato.Core.Models;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Entities.ViewProviders
{
    public class HomeViewProvider : ViewProviderBase<HomeIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            
            // Build view model
            var coreIndexViewModel = new CoreIndexViewModel()
            {
                Latest = new EntityIndexViewModel<Entity>()
                {
                    Options = new EntityIndexOptions()
                    {
                        Sort = SortBy.LastReply,
                        NoResultsText = "No new contributions"
                    },
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false,
                        CountTotal = false
                    }
                },
                Popular = new EntityIndexViewModel<Entity>()
                {
                    Options = new EntityIndexOptions()
                    {
                        Sort = SortBy.Popular,
                        NoResultsText = "No popular contributions"
                    },
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false,
                        CountTotal = false
                    }
                }
            };

            // Build view
            return Task.FromResult(Views(
                View<CoreIndexViewModel>("Core.Entities.Index.Content", model => coreIndexViewModel)
                    .Zone("content").Order(2)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult)); ;
        }

    }

}
