namespace PlatoCore.Layout.Views.Abstractions
{
    public class ViewDescriptor
    {

        public string Name { get; set; }

        public IView View { get; set; }

        public ViewDescriptor(IView view)
        {
            Name = view.ViewName;
            View = view;
        }

    }

}
