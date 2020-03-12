using System.Collections.Generic;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapterCollection
    {     

        IList<ITagHelperAdapter> Adapters { get; }

    }

    public class TagHelperAdapterCollection : ITagHelperAdapterCollection
    {
        public IList<ITagHelperAdapter> Adapters { get; }

        public TagHelperAdapterCollection()
        {
            Adapters = new List<ITagHelperAdapter>();
        }

    }

    public static class TagHelperAdapterCollectionExtensions
    {
        
        public static void Add(this ITagHelperAdapterCollection tagHelperAdapterCollection, IEnumerable<ITagHelperAdapter> adapters)
        {           
            foreach (var adapter in adapters)
            {
                tagHelperAdapterCollection.Adapters.Add(adapter);
            }            
        }

        public static void Add(this ITagHelperAdapterCollection tagHelperAdapterCollection, ITagHelperAdapter adapter)
        {
            tagHelperAdapterCollection.Adapters.Add(adapter);
        }

        public static IEnumerable<ITagHelperAdapter> First(this ITagHelperAdapterCollection tagHelperAdapterCollection, string viewName, string tagId)
        {

            if (tagHelperAdapterCollection.Adapters == null)
            {
                return null;
            }

            if (tagHelperAdapterCollection.Adapters.Count <= 0)
            {
                return null;
            }

            IList<ITagHelperAdapter> output = null;
            foreach (var adapter in tagHelperAdapterCollection.Adapters)
            {
                var isView = adapter.ViewName?.Equals(viewName, System.StringComparison.OrdinalIgnoreCase) ?? false;
                var isTag = adapter.TagHelperId?.Equals(tagId, System.StringComparison.OrdinalIgnoreCase) ?? false;
                if (isView && isTag)
                {
                    if (output == null)
                    {
                        output = new List<ITagHelperAdapter>();
                    }
                    output.Add(adapter);
                }
            }
            return output;
        }

        public static IEnumerable<ITagHelperAdapter> FirstOrDefault(
            this ITagHelperAdapterCollection tagHelperAdapterCollection, string viewName, string tagId)
        {
            var first = tagHelperAdapterCollection.First(viewName, tagId);
            return first ?? default(List<ITagHelperAdapter>);
        }

    }

}
