namespace PlatoCore.Navigation.Abstractions
{
    public interface INavigationProvider
    {
        void BuildNavigation(string name, INavigationBuilder builder);

    }
}
