﻿using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations
{
    public interface IMigrationProvider
    {

        PreparedMigration GetSchema(string version);

        List<PreparedMigration> Schemas { get; }

        IMigrationProvider LoadSchemas(List<string> versions);

    }
}