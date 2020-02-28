using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace Plato.Attachments.Stores
{

    #region "AttachmentQuery"

    public class AttachmentQuery : DefaultQuery<Models.Attachment>
    {

        private readonly IQueryableStore<Models.Attachment> _store;

        public AttachmentQuery(IQueryableStore<Models.Attachment> store)
        {
            _store = store;
        }

        public AttachmentQueryParams Params { get; set; }

        public override IQuery<Models.Attachment> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (AttachmentQueryParams)Convert.ChangeType(defaultParams, typeof(AttachmentQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Models.Attachment>> ToList()
        {

            var builder = new AttachmentQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var contentGuid = Params.ContentGuid.Value ?? string.Empty;
            var keywords = Params.Keywords.Value ?? string.Empty;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("ContentGuid", DbType.String, contentGuid),
                new DbParam("Keywords", DbType.String, keywords)
            });

        }
        
    }

    #endregion

    #region "AttachmentQueryParams"

    public class AttachmentQueryParams
    {
        
        private WhereInt _id;
        private WhereString _contentGuid;
        private WhereString _keywords;
        
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString ContentGuid
        {
            get => _contentGuid ?? (_contentGuid = new WhereString());
            set => _contentGuid = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

    }

    #endregion

    #region "AttachmentQueryBuilder"

    public class AttachmentQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _attachmentsTableName;

        private readonly AttachmentQuery _query;

        public AttachmentQueryBuilder(AttachmentQuery query)
        {
            _query = query;
            _attachmentsTableName = GetTableNameWithPrefix("Attachments");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            // Order only if we have something to order by
            sb.Append(" ORDER BY ").Append(!string.IsNullOrEmpty(orderBy)
                ? orderBy
                : "(SELECT NULL)");
            // Limit results only if we have a specific page size
            if (!_query.IsDefaultPageSize)
                sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            if (!_query.CountTotal)
                return string.Empty;
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(a.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        private string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb
                .Append("a.Id, ")
                .Append("a.[Name], ")
                .Append("CAST(1 AS BINARY(1)) AS ContentBlob, ") // for perf not returned with paged results
                .Append("a.ContentType, ")
                .Append("a.ContentLength, ")
                .Append("a.ContentGuid, ")
                .Append("a.TotalViews, ")
                .Append("a.CreatedUserId, ")
                .Append("a.CreatedDate, ")
                .Append("a.ModifiedUserId, ")
                .Append("a.ModifiedDate ");
            return sb.ToString();
        }

        private string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_attachmentsTableName)
                .Append(" a ");
            return sb.ToString();
        }

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("a.Id"));
            }

            // ContentGuid
            if (!String.IsNullOrEmpty(_query.Params.ContentGuid.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ContentGuid.Operator);
                sb.Append(_query.Params.ContentGuid.ToSqlString("a.ContentGuid", "ContentGuid"));
            }


            return sb.ToString();

        }

        private string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "a." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        #endregion

    }

    #endregion

}
