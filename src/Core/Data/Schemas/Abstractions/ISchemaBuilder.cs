using PlatoCore.Data.Schemas.Abstractions.Builders;

namespace PlatoCore.Data.Schemas.Abstractions
{

    public interface ISchemaBuilder : ISchemaBuilderBase
    {

        ITableBuilder TableBuilder { get; }

        IProcedureBuilder ProcedureBuilder { get; }

        IFullTextBuilder FullTextBuilder { get; }

        IIndexBuilder IndexBuilder { get;  }

    }

}
