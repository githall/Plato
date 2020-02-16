using System;
using System.Text;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Data.Schemas.Abstractions.Builders;
using PlatoCore.Text.Abstractions;

namespace PlatoCore.Data.Schemas.Builders
{

    // This implementation is simple and only intended to provides a
    // very thin wrapper around create, alter and delete for non-unique clustered indexes.
    // PRIMARY KEY or unique constraints should be handled separately

    // CREATE INDEX
    //https://docs.microsoft.com/en-us/sql/t-sql/statements/create-index-transact-sql?view=sql-server-2017

    // ALTER INDEX
    // https://docs.microsoft.com/en-us/sql/t-sql/statements/alter-index-transact-sql?view=sql-server-2017

    // DROP INDEX
    // https://docs.microsoft.com/en-us/sql/t-sql/statements/drop-index-transact-sql?view=sql-server-2017

    public class IndexBuilder : SchemaBuilderBase, IIndexBuilder
    {
        public IndexBuilder(
            IDbContext dbContext,
            IPluralize pluralize) : base(dbContext, pluralize)
        {
        }

        public IIndexBuilder CreateIndex(SchemaIndex index)
        {

            if (Options.DropIndexesBeforeCreate)
            {
                DropIndex(index);
            }

            AddStatement(CreateIndexInternal(index));
            return this;

        }

        public IIndexBuilder AlterIndex(SchemaIndex index)
        {
            AddStatement(AlterIndexInternal(index));
            return this;
        }

        public IIndexBuilder DropIndex(SchemaIndex index)
        {
            AddStatement(DropIndexInternal(index));
            return this;
        }

        // -------------------------------
        
        private string CreateIndexInternal(SchemaIndex index)
        {

            // IF NOT EXISTS (SELECT object_id FROM sys.indexes WHERE [name] = 'prefix_IX_tableName_firstColumnName')
            // BEGIN
            //      CREATE INDEX prefix_IX_tableName_firstColumnName ON tableName
            //          ( columnName(s) )
            //      WITH(FILLFACTOR = 30)
            // END

            var sb = new StringBuilder();

            var indexName = index.GenerateName();

            // Ensure the index does not already exist

            sb.Append("IF NOT EXISTS (SELECT object_id FROM sys.indexes WHERE [name] = '")
                .Append(PrependTablePrefix(indexName))
                .Append("')")
                .Append(NewLine);

            sb.Append("BEGIN")
                 .Append(NewLine); ;

            // Create index

            sb.Append("CREATE INDEX ")
                .Append(PrependTablePrefix(indexName))
                .Append(" ON ")
                .Append(PrependTablePrefix(index.TableName))
                .Append(" (")
                .Append(index.Columns.ToDelimitedString(','))
                .Append(")");

            if (index.FillFactor > 0)
            {
                sb.Append(" WITH (FILLFACTOR = ")
                    .Append(index.FillFactor)
                    .Append(")");
            }

            sb.Append(";");

            // End index exists check
            sb
                .Append(NewLine)
                .Append("END");        

            return sb.ToString();

        }

        private string AlterIndexInternal(SchemaIndex index)
        {

            // Defragment by physically removing rows that have been logically deleted from the table, and merging rowgroups.  
    
            var indexName = index.GenerateName();

            var sb = new StringBuilder();
            sb.Append("ALTER INDEX ")
                .Append(PrependTablePrefix(indexName))
                .Append(" ON ")
                .Append(PrependTablePrefix(index.TableName))
                .Append(" REORGANIZE");

            return sb.ToString();

        }
        
        private string DropIndexInternal(SchemaIndex index)
        {

            // DROP INDEX IX_tableName_firstColumn ON tableName
            // GO
            
            var indexName = index.GenerateName();

            var sb = new StringBuilder();
            sb.Append("DROP INDEX ")
                .Append(PrependTablePrefix(indexName))
                .Append(" ON ")
                .Append(PrependTablePrefix(index.TableName))
                .Append(";")
                .Append(NewLine);

            return sb.ToString();

        }

    }
    
}
