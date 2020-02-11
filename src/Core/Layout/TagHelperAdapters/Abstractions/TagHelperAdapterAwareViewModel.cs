namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapterAwareViewModel
    {
        ITagHelperAdapterCollection TagHelperAdapters { get; set; }
    }

    public class TagHelperAdapterAwareViewModel : ITagHelperAdapterAwareViewModel
    {

        public ITagHelperAdapterCollection TagHelperAdapters { get; set; }

    }

}
