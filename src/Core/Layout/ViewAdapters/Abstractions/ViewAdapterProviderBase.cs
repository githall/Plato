using System;
using System.Threading.Tasks;

namespace PlatoCore.Layout.ViewAdapters.Abstractions
{

    public abstract class ViewAdapterProviderBase : IViewAdapterProvider
    {

        public string ViewName { get; set; }

        public abstract Task<IViewAdapterResult> ConfigureAsync(string viewName);


        public IViewAdapterResult Adapt(string viewName,
            Action<IViewAdapterBuilder> configure)
        {

            // Apply adapter builder & return compiled results
            var builder = new ViewAdapterBuilder(viewName);
            configure(builder);

            // Ensure results are aware of the builder that created them
            var result = builder.ViewAdapterResult;
            result.Builder = builder;

            // Return results
            return result;

        }

        public Task<IViewAdapterResult> AdaptAsync(string viewName,
            Action<IViewAdapterBuilder> configure)
        {     
            return Task.FromResult(Adapt(viewName, configure));
        }

    }

}
