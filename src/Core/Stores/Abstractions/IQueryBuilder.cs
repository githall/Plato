namespace PlatoCore.Stores.Abstractions
{
    public interface IQueryBuilder
    {

        string BuildSqlPopulate();

        string BuildSqlCount();

    }

}
