using System.Collections.Generic;

namespace PlatoCore.Data.Schemas.Abstractions
{
    public class SchemaFullTextIndex
    {

        public string TableName { get; set; }

        public string PrimaryKeyName { get; set; }

        public ICollection<SchemaFullTextColumn> Columns { get; set; }

        public short FillFactor { get; set; } = 30;

        public string CatalogName { get; set; }

    }

    public class SchemaFullTextColumn
    {

        public string ColumnName { get; set; }

        public string TypeColumnName { get; set; }

        public int LanguageCode { get; set; } = 1033;

        public SchemaFullTextColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public SchemaFullTextColumn(
            string columnName,
            string typeColumnName)
            : this(columnName)
        {
            TypeColumnName = typeColumnName;
        }

        public SchemaFullTextColumn(
            string columnName,
            string typeColumnName,
            int languageCode)
            : this(columnName, typeColumnName)
        {
            LanguageCode = languageCode;
        }

    }

}
