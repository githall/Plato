using System.Data;

namespace PlatoCore.Abstractions
{

    public interface IDbModel
    {
        void PopulateModel(IDataReader dr);
    }

    public interface IDbModel<T> : IDbModel where T : class 
    {
        // TODO: Deprecated - to be removed
    }

}
