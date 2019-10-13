namespace Plato.Internal.Layout.Views
{
    public class ViewPosition
    {

        public string Zone { get; set; }

        public int Order { get; set; }

        public ViewPosition(string zone, int order)
        {
            Zone = zone;
            Order = order;
        }

    }

}
