using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.ViewAdapters.Abstractions;

namespace PlatoCore.Layout.ViewAdapters
{

    public class ViewAdapterBuilder : IViewAdapterBuilder
    {

        public string ViewName { get; }

        private readonly IViewAdapterResult _viewAdapterResult;

        public IViewAdapterResult ViewAdapterResult => _viewAdapterResult;

        public ViewAdapterBuilder(string viewName) 
        {
            _viewAdapterResult = new ViewAdapterResult();
            ViewName = viewName;
        }
        
        public IViewAdapterBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> alteration)
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            _viewAdapterResult.OutputAlterations.Add(alteration);
            return this;
        }

        public IViewAdapterBuilder AdaptView(string viewName)
        {
            if (viewName == null)
            {
                throw new NullReferenceException(nameof(viewName));
            }

            _viewAdapterResult.ViewAlterations.Add(viewName);
            return this;
        }

        public IViewAdapterBuilder AdaptView(string[] viewNames)
        {
            foreach (var viewName in viewNames)
            {
                AdaptView(viewName);
            }
            return this;
        }

        public IViewAdapterBuilder AdaptModel<TModel>(Func<TModel, Task<object>> alteration) where TModel : class
        {

            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            // Convert TModel to object for storage within adapter result
            var typedDelegate = new Func<object, Task<object>>(async (object model) =>
            {

                // For view component model adapters, the convention is to use the first anonymous
                // type argument matching our TModel. This is not perfect but at last allows 
                // adapters that adapt view components to use a strong typing
                // To modify anonymous type arguments without the generic typing
                // use the regular non generic AdaptModel method below instead
                // TODO: Possibly implement a convention-based approach for greater extensibility. 
                // For example ViewComponentModelAdapterConvention : IModelAdapterConvention
                if (IsComponent(model))
                {

                    foreach (var argument in PrepareAnonymousTypeArguments(model))
                    {
                        if (argument is TModel)
                        {
                            return await alteration((TModel)argument);
                        }
                    }

                    // IF we reach this point we have not found an anonymous type 
                    // argument matching the model we are attempting to adapt

                }

                // Patial views
                if (model is TModel)
                {
                    return await alteration((TModel)model);
                }

                return null;

            });

            _viewAdapterResult.ModelAlterations.Add(typedDelegate);
            return this;
        }

      
        public IViewAdapterBuilder AdaptModel(Func<object, Task<object>> alteration)
        {

            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            _viewAdapterResult.ModelAlterations.Add(alteration);
            return this;

        }

        // ----------------

        bool IsComponent(object model)
        {

            // We need a model to inspect
            if (model == null)
            {
                return false;
            }

            return model.GetType().IsAnonymousType();

        }

        private IEnumerable<object> PrepareAnonymousTypeArguments(object model)
        {
            var arguments = new List<object>();
            var properties = TypeDescriptor.GetProperties(model);
            foreach (PropertyDescriptor property in properties)
            {
                arguments.Add(property.GetValue(model));
            }
            return arguments;
        }


    }

}
