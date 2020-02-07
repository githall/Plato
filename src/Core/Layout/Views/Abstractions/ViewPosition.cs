namespace PlatoCore.Layout.Views.Abstractions
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
