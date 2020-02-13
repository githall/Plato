
namespace PlatoCore.Layout.Views.Abstractions
{

    public interface IView
    {

        string ViewName { get; set; }

        object Model { get; set; }

    }

    public class View : IView
    {

        public string ViewName { get; set; }
        
        public object Model { get; set; }

        public View()
        {           
        }

        public View(string viewName, object model = null)
        {
            ViewName = viewName;
            Model = model;
        }

    }

}
