using System.Collections.Generic;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapterCollection
    {     

        IList<ITagHelperAdapter> Adapters { get; }

    }

    public class TagHelperAdapterCollection : ITagHelperAdapterCollection
    {

        private readonly IList<ITagHelperAdapter> _adapters;

        public IList<ITagHelperAdapter> Adapters => _adapters;

        public TagHelperAdapterCollection()
        {
            _adapters = new List<ITagHelperAdapter>();
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

        public static IEnumerable<ITagHelperAdapter> First(this ITagHelperAdapterCollection tagHelperAdapterCollection, string id)
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
                if (adapter.Id.Equals(id, System.StringComparison.OrdinalIgnoreCase))
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
            this ITagHelperAdapterCollection tagHelperAdapterCollection, string id)
        {
            var first = tagHelperAdapterCollection.First(id);
            return first ?? default(List<ITagHelperAdapter>);
        }

    }

}
