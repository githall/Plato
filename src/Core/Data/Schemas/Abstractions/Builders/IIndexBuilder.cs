﻿namespace PlatoCore.Data.Schemas.Abstractions.Builders
{
    public interface IIndexBuilder : ISchemaBuilderBase
    {

        IIndexBuilder CreateIndex(SchemaIndex index);

        IIndexBuilder AlterIndex(SchemaIndex index);

        IIndexBuilder DropIndex(SchemaIndex index);

    }

}
