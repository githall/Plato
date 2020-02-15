namespace PlatoCore.Layout.Views.Abstractions
{
    public interface IPositionedView : IView
    {

        ViewPosition Position { get; }

        IPositionedView Zone(string zone);

        IPositionedView Order(int order);

    }

}
