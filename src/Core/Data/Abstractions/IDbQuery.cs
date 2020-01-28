
namespace PlatoCore.Data.Abstractions
{
 
    public interface IDbQueryConfiguration
    {
        IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class;
    }

}
