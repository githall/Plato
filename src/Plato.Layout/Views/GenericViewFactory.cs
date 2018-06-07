﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Layout.Views
{

    public interface IGenericViewFactory
    {
        Task<GenericViewDescriptor> CreateAsync(string key, object view);

        Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext);

    }
    
    public class GenericViewFactory  : IGenericViewFactory
    {

        private readonly IGenericViewTableManager _genericViewTableManager;
        private readonly IGenericViewInvoker _genericViewInvoker;

        public GenericViewFactory(
            IGenericViewTableManager genericViewTableManager,
            IGenericViewInvoker genericViewInvoker)
        {
            _genericViewTableManager = genericViewTableManager;
            _genericViewInvoker = genericViewInvoker;
        }

        public async Task<GenericViewDescriptor> CreateAsync(string name, object model)
        {
            return await _genericViewTableManager.TryAdd(name, model);
        }

        public async Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext)
        {

            // Contextulize generic view invoker

            _genericViewInvoker.Contextualize(displayContext);
            
            // Apply view & model alterations

            if (displayContext.viewAdaptorResults != null)
            {
                foreach (var viewAdaptorResult in displayContext.viewAdaptorResults)
                {

                    // ensure generic view
                    if (displayContext.ViewDescriptor.Value is IGenericView genericView)
                    {
                        // view alterations
                        var viewAlterations = viewAdaptorResult.ViewAlterations;
                        if (viewAlterations.Count > 0)
                        {
                            foreach (var alteration in viewAlterations)
                            {
                                genericView.ViewName = alteration;
                            }
                        }

                        // model alterations
                        var modelAlterations = viewAdaptorResult.ModelAlterations;
                        if (modelAlterations.Count > 0)
                        {
                            foreach (var alteration in modelAlterations)
                            {
                                genericView.Model = alteration(genericView.Model);
                            }
                        }

                        displayContext.ViewDescriptor.Value = genericView;
                    }

                }

            }

            // Invoke generic view

            var htmlContent = await _genericViewInvoker.InvokeAsync(displayContext.ViewDescriptor);
            
            // Apply adaptor output alterations

            if (displayContext.viewAdaptorResults != null)
            {
                foreach (var viewAdaptorResult in displayContext.viewAdaptorResults)
                {
                    var alterations = viewAdaptorResult.OutputAlterations;
                    if (alterations.Count > 0)
                    {
                        foreach (var alteration in alterations)
                        {
                            htmlContent = alteration(htmlContent);
                        }
                    }
                
                }
            }
          
            return htmlContent;

        }
        }
}
