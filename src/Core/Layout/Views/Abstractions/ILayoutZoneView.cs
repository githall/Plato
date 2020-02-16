namespace PlatoCore.Layout.Views.Abstractions
{

    /// <summary>
    /// Represents a view that can be positioned within the layout. 
    /// </summary>
    public interface ILayoutZoneView : IView
    {

        ViewPosition Position { get; }

        ILayoutZoneView Zone(string zone);

        ILayoutZoneView Order(int order);

    }

}
