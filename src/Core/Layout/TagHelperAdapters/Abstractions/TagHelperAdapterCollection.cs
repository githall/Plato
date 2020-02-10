using System.Collections.Generic;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapterCollection
    {
        void Add(IEnumerable<ITagHelperAdapter> adapters);

        void Add(ITagHelperAdapter adapter);

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

        public void Add(IEnumerable<ITagHelperAdapter> adapters)
        {           
            foreach (var adapter in adapters)
            {
                Add(adapter);
            }            
        }

        public void Add(ITagHelperAdapter adapter)
        {          
            _adapters.Add(adapter);
        }

    }

    public static class TagHelperAdapterCollectionExtensions
    {

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
            foreach (var alteration in tagHelperAdapterCollection.Adapters)
            {
                if (alteration.Id.Equals(id, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (output == null)
                    {
                        output = new List<ITagHelperAdapter>();
                    }
                    output.Add(alteration);
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
