namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapterAwareViewModel
    {
        ITagHelperAdapterCollection TagHelperAdapters { get; set; }
    }

    public class TagHelperAdapterAwareViewModel : ITagHelperAdapterAwareViewModel
    {

        private ITagHelperAdapterCollection _tagHelperAdapters;

        public ITagHelperAdapterCollection TagHelperAdapters {
            get
            {
                if (_tagHelperAdapters == null)
                {
                    _tagHelperAdapters = new TagHelperAdapterCollection();
                }
                return _tagHelperAdapters;
            }
            set
            {
                _tagHelperAdapters = value;
            }

        }

    }

}
