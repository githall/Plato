using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Emails.Abstractions;

namespace Plato.Email.Stores
{

    #region "EmailAttachmentQuery"

    public class EmailAttachmentQuery : DefaultQuery<EmailAttachment>
    {

        public IFederatedQueryManager<EmailAttachment> FederatedQueryManager { get; set; }

        public IQueryAdapterManager<EmailAttachment> QueryAdapterManager { get; set; }

        private readonly IQueryableStore<EmailAttachment> _store;

        public EmailAttachmentQueryBuilder Builder { get; private set; }

        public EmailAttachmentQuery(IQueryableStore<EmailAttachment> store)
        {
            _store = store;
        }

        public EmailAttachmentQueryParams Params { get; set; }

        public override IQuery<EmailAttachment> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EmailAttachmentQueryParams)Convert.ChangeType(defaultParams, typeof(EmailAttachmentQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EmailAttachment>> ToList()
        {

            Builder = new EmailAttachmentQueryBuilder(this);
            var populateSql = Builder.BuildSqlPopulate();
            var countSql = Builder.BuildSqlCount();     
            var keywords = Params.Keywords.Value ?? string.Empty;

            return await _store.SelectAsync(new IDbDataParameter[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Keywords", DbType.String, keywords)
            });

        }

    }

    #endregion

    #region "EmailAttachmentQueryParams"

    public class EmailAttachmentQueryParams
    {
        
        private WhereInt _id;
        private WhereInt _emailId;
        private WhereString _keywords;
        
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt EmailId
        {
            get => _emailId ?? (_emailId = new WhereInt());
            set => _emailId = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

    }

    #endregion

    #region "EmailAttachmentQueryBuilder"

    public class EmailAttachmentQueryBuilder : IQueryBuilder
    {

        #region "Constructor"   

        private readonly string _attachmentTableName;

        private readonly EmailAttachmentQuery _query;

        public EmailAttachmentQueryBuilder(EmailAttachmentQuery query)
        {
            _query = query;        
            _attachmentTableName = GetTableNameWithPrefix("EmailAttachments");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhere();
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
            var whereClause = BuildWhere();
            var sb = new StringBuilder();      
            sb.Append("SELECT COUNT(ea.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        public string Where => _where ?? (_where = BuildWhere());

        #endregion

        #region "Private Methods"

        private string _where = null;

        private string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("ea.*");

            // -----------------
            // Apply any select query adapters
            // -----------------

            _query.QueryAdapterManager?.BuildSelect(_query, sb);

            return sb.ToString();
        }

        private string BuildTables()
        {
            var sb = new StringBuilder();
            sb
                .Append(_attachmentTableName)
                .Append(" ea ");

            // -----------------
            // Apply any table query adapters
            // -----------------

            _query.QueryAdapterManager?.BuildTables(_query, sb);

            return sb.ToString();
        }

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhere()
        {
            var sb = new StringBuilder();

            // -----------------
            // Apply any where query adapters
            // -----------------

            _query.QueryAdapterManager?.BuildWhere(_query, sb);

            // -----------------
            // Ensure we have params
            // -----------------

            if (_query.Params == null)
            {
                return string.Empty;
            }

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append("(").Append(_query.Params.Id.ToSqlString("ea.Id")).Append(")");
            }

            // EmailId
            if (_query.Params.EmailId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EmailId.Operator);
                sb.Append("(").Append(_query.Params.EmailId.ToSqlString("ea.EmailId")).Append(")");
            }

            // Keywords
            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("ea.[Name]", "Keywords"));
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
                : "ea." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in GetSafeSortColumns())
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

        private IDictionary<string, OrderBy> GetSafeSortColumns()
        {
            var output = new Dictionary<string, OrderBy>();
            foreach (var sortColumn in _query.SortColumns)
            {
                var columnName = GetSortColumn(sortColumn.Key);
                if (String.IsNullOrEmpty(columnName))
                {
                    throw new Exception($"No sort column could be found for the supplied key of '{sortColumn.Key}'");
                }
                output.Add(columnName, sortColumn.Value);

            }

            return output;
        }


        private string GetSortColumn(string columnName)
        {

            if (String.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            switch (columnName.ToLowerInvariant())
            {
                case "id":
                    return "ea.Id";      
                case "name":
                    return "ea.[Name]";
                case "type":
                    return "ea.ContentType";
                case "size":
                    return "ea.ContentLength";
                case "uniqueness":
                    return "ea.ContentCheckSum";              
                case "createduserid":
                    return "ea.CreatedUserId";
                case "created":
                    return "ea.CreatedDate";
                case "createddate":
                    return "ea.CreatedDate";
            }

            return string.Empty;

        }

        #endregion

    }

    #endregion

}
