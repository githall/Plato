using System.Collections.Generic;

namespace PlatoCore.Search.Abstractions
{
    public class FullTextIndex
    {

        public string TableName { get; set; }

        public IEnumerable<FullTextColumn> Columns { get; set; }

        public short FillFactor { get; set; } = 30;

        protected FullTextIndex(string tableName)
        {
            TableName = tableName;
        }

        public FullTextIndex(string tableName, string[] columns) : this(tableName)
        {
            var list = new List<FullTextColumn>();
            foreach (var column in columns)
            {
                list.Add(new FullTextColumn(column));
            }
            Columns = list;
        }

        public FullTextIndex(string tableName, IEnumerable<FullTextColumn> columns) : this(tableName)
        {
            Columns = columns;
        }

        public FullTextIndex(
            string tableName,
            IEnumerable<FullTextColumn> columns,         
            short fillFactor) : this(tableName, columns)
        {
            FillFactor = fillFactor;
        }
        
    }

    public class FullTextColumn
    {

        public string ColumnName { get; set; }

        public string TypeColumnName { get; set; }

        public int LanguageCode { get; set; } = 1033;

        public FullTextColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public FullTextColumn(
            string columnName,
            string typeColumnName)
            :this(columnName)
        {        
            TypeColumnName = typeColumnName;
        }

        public FullTextColumn(
            string columnName,
            string typeColumnName,
            int languageCode)
            : this(columnName, typeColumnName)
        {
            LanguageCode = languageCode;
        }

    }

}
