using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.Views
{

    public interface IView
    {
        IEmbeddedView EmbeddedView { get; set; }

        string ViewName { get; set; }

        object Model { get; set; }

    }

    public class View : IView
    {

        public IEmbeddedView EmbeddedView { get; set; }
        
        public string ViewName { get; set; }
        
        public object Model { get; set; }
        
        public View(IEmbeddedView view)
        {
            ViewName = view.GetType().Name;
            EmbeddedView = view;
        }

        public View(string viewName, object model = null)
        {
            ViewName = viewName;
            Model = model;
        }

    }

}
