using System;
using System.Threading.Tasks;

namespace PlatoCore.Layout.ViewAdapters
{

    public abstract class ViewAdapterProviderBase : IViewAdapterProvider
    {

        public string ViewName { get; set; }

        public abstract Task<IViewAdapterResult> ConfigureAsync(string viewName);

        public Task<IViewAdapterResult> AdaptAsync(string viewName,
            Action<IViewAdapterBuilder> configure)
        {

            // Apply adapter builder & return compiled results
            var builder = new ViewAdapterBuilder(viewName);
            configure(builder);

            // Ensure results are aware of the builder that created them
            var result = builder.ViewAdapterResult;
            result.Builder = builder;

            // Return results
            return Task.FromResult(result);

        }

    }

}
