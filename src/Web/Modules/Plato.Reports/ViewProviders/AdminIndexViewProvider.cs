﻿using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Admin.Models;
using Plato.Reports.Services;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewProviders
{

    public class AdminIndexViewProvider : ViewProviderBase<AdminIndex>
    {
        
        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(IDateRangeStorage dateRangeStorage)
        {
            _dateRangeStorage = dateRangeStorage;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
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
                //View<ReportOptions>("Reports.Admin.Index.Tools", model => reportIndexOptions).Zone("header-right")
                //    .Order(int.MinValue),
                View<ReportOptions>("Reports.AdminIndex", model => reportIndexOptions).Zone("content")
                    .Order(int.MinValue + 10)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel,
            IViewProviderContext context)
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
