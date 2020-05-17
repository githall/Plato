﻿using System;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Metrics.Models;
using Plato.Reports.ViewModels;
using Plato.Reports.PageViews.Models;

namespace Plato.Reports.PageViews.ViewProviders
{

    public class AdminViewProvider : ViewProviderBase<PageViewIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(PageViewIndex viewModel, IViewProviderContext context)
        {

            // Get view model from context
            var indexViewModel =
                context.Controller.HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] as ReportIndexViewModel<Metric>;
            
            // Ensure we have the view model
            if (indexViewModel == null)
            {
                throw new Exception("No type of ReportIndexViewModel has been registered with HttpContext.Items");
            }

            return Task.FromResult(Views(
                View<ReportIndexViewModel<Metric>>("Admin.Index.Header", model => indexViewModel).Zone("header").Order(1),
                View<ReportOptions>("Reports.Admin.Index.Tools", model => indexViewModel.Options).Zone("header-right").Order(1),
                View<ReportIndexViewModel<Metric>>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            ));

        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(PageViewIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(PageViewIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PageViewIndex viewModel, IViewProviderContext context)
        {

            var model = new ReportOptions();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            return await BuildIndexAsync(viewModel, context);

        }

    }

}
