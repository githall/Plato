﻿using System;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Metrics.Models;
using Plato.Reports.Models;
using Plato.Reports.Services;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<ReportIndex>
    {

        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminViewProvider(IDateRangeStorage dateRangeStorage)
        {
            _dateRangeStorage = dateRangeStorage;
        }


        public override Task<IViewProviderResult> BuildIndexAsync(ReportIndex viewModel, IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
         
            // Build index view model
            var reportIndexOptions = new ReportOptions()
            {
                Start = range.Start,
                End = range.End
            };
            
            return Task.FromResult(Views(
                View<ReportOptions>("Admin.Index.Header", model => reportIndexOptions).Zone("header").Order(1),
                View<ReportOptions>("Admin.Index.Tools", model => reportIndexOptions).Zone("header-right").Order(1),
                View<ReportOptions>("Admin.Index.Content", model => reportIndexOptions).Zone("content").Order(1)
            ));

        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(ReportIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(ReportIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(ReportIndex viewModel, IViewProviderContext context)
        {
            var model = new ReportOptions();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {
                var storage = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
                storage.Set(model.Start, model.End);
            }

            return await BuildIndexAsync(viewModel, context);
        }

    }

}
