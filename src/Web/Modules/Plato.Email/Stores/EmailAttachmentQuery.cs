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

        private readonly string _shellFeaturesTableName;       
        private readonly string _usersTableName;
        private readonly string _attachmentTableName;

        private readonly EmailAttachmentQuery _query;

        public EmailAttachmentQueryBuilder(EmailAttachmentQuery query)
        {
            _query = query;
            _shellFeaturesTableName = GetTableNameWithPrefix("ShellFeatures");
            _usersTableName = GetTableNameWithPrefix("Users");
            _attachmentTableName = GetTableNameWithPrefix("EmailAttachments");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhere();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("DECLARE @MaxRank int;")
            .Append(Environment.NewLine)
            .Append(BuildFederatedResults())
            .Append(Environment.NewLine);
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
            sb.Append("DECLARE @MaxRank int;")
            .Append(Environment.NewLine)
            .Append(BuildFederatedResults())
            .Append(Environment.NewLine);
            sb.Append("SELECT COUNT(f.Id) FROM ")
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
                sb.Append("(").Append(_query.Params.Id.ToSqlString("f.Id")).Append(")");
            }

            // EmailId
            if (_query.Params.EmailId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EmailId.Operator);
                sb.Append("(").Append(_query.Params.Id.ToSqlString("ea.EmailId")).Append(")");
            }

            // Keywords
            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb.Append(_query.Params.Keywords.ToSqlString("f.[Name]", "Keywords"));
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
                : "f." + columnName;
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
                    return "f.Id";
                case "featureid":
                    return "f.FeatureId";
                case "name":
                    return "f.[Name]";
                case "type":
                    return "f.ContentType";
                case "size":
                    return "f.ContentLength";
                case "uniqueness":
                    return "f.ContentCheckSum";
                case "views":
                    return "f.TotalViews";
                case "totalviews":
                    return "f.TotalViews";
                case "createduserid":
                    return "f.CreatedUserId";
                case "created":
                    return "f.CreatedDate";
                case "createddate":
                    return "f.CreatedDate";
            }

            return string.Empty;

        }


        // -- Search

        string BuildFederatedResults()
        {

            // No keywords
            if (string.IsNullOrEmpty(GetKeywords()))
            {
                return string.Empty;
            }

            // Build standard SQL or full text queries
            var sb = new StringBuilder();

            // Compose federated queries
            var queries = _query.FederatedQueryManager.GetQueries(_query);

            // Create a temporary table for all our federated queries
            sb.Append("DECLARE @temp TABLE (Id int, [Rank] int); ");

            // Execute each federated query adding results to temporary table
            foreach (var query in queries)
            {
                sb.Append("INSERT INTO @temp ")
                    .Append(Environment.NewLine)
                    .Append(query)
                    .Append(Environment.NewLine);
            }

            // Build final distinct and aggregated results from federated results
            sb.Append("DECLARE @results TABLE (Id int, [Rank] int); ")
                .Append(Environment.NewLine)
                .Append("INSERT INTO @results ")
                .Append(Environment.NewLine)
                .Append("SELECT Id, SUM(Rank) FROM @temp GROUP BY Id;")
                .Append(Environment.NewLine);

            // Get max / highest rank from final results table
            sb.Append("SET @MaxRank = ")
                .Append(_query.Options.SearchType != SearchTypes.Tsql
                    ? "(SELECT TOP 1 [Rank] FROM @results ORDER BY [Rank] DESC)"
                    : "0")
                .Append(";");

            return sb.ToString();

        }

        bool HasKeywords()
        {
            return !string.IsNullOrEmpty(GetKeywords());
        }

        string GetKeywords()
        {

            if (string.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                return string.Empty;
            }

            return _query.Params.Keywords.Value;

        }

        #endregion

    }

    #endregion

}
