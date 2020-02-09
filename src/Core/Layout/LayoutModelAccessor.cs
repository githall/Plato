using System.Threading;

namespace PlatoCore.Layout
{

    public interface ILayoutModelAccessor
    {
        LayoutViewModel LayoutViewModel { get; set; }
    }

    public class LocalLayoutModelAccessor : ILayoutModelAccessor
    {
        private readonly AsyncLocal<LayoutViewModel> _storage = new AsyncLocal<LayoutViewModel>();

        public LayoutViewModel LayoutViewModel
        {
            get => _storage.Value;
            set => _storage.Value = value;
        }
    }

}
