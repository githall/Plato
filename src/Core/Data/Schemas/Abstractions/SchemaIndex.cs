using System.Text;

namespace PlatoCore.Data.Schemas.Abstractions
{

    public class SchemaIndex
    {

        public string TableName { get; set; }

        public string[] Columns { get; set; }

        public short FillFactor { get; set; } = 30;

        public string GenerateName()
        {

            // Get first column
            // Stripping any reserved word characters
            var columnName = string.Empty;
            foreach (var column in Columns)
            {
                columnName = column
                    .Replace("[", "")
                    .Replace("]", "");
                break;
            }

            // Auto generate name combining table name with first column name            
            var sb = new StringBuilder();
            sb.Append("IX_").Append(this.TableName);
            if (!string.IsNullOrEmpty(columnName))
            {
                sb.Append("_").Append(columnName);
            }

            // I.e. IX_{tableName}_{firstColumnName}
            return sb.ToString();

        }

    }

}
