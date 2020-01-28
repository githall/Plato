using System;
using System.Collections.Generic;

namespace PlatoCore.Data.Schemas.Abstractions.Builders
{
    public interface ISchemaBuilderBase : IDisposable
    {

        ICollection<string> Statements { get; }
        
        ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure);

    }

}
