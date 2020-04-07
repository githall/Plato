using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Files.Sharing.Models;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace Plato.Files.Sharing.Stores
{

    #region "FileInviteQuery"

    public class FileInviteQuery : DefaultQuery<Models.FileInvite>
    {

        private readonly IQueryableStore<FileInvite> _store;

        public FileInviteQuery(IQueryableStore<Models.FileInvite> store)
        {
            _store = store;
        }

        public FileInviteQueryParams Params { get; set; }

        public override IQuery<FileInvite> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (FileInviteQueryParams)Convert.ChangeType(defaultParams, typeof(FileInviteQueryParams));
            return this;
        }

        public override async Task<IPagedResults<Models.FileInvite>> ToList()
        {

            var builder = new FileInviteQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var email = Params.Email.Value ?? string.Empty;

            return await _store.SelectAsync(
                new IDbDataParameter[]
                {
                    new DbParam("PageIndex", DbType.Int32, PageIndex),
                    new DbParam("PageSize", DbType.Int32, PageSize),
                    new DbParam("SqlPopulate", DbType.String, populateSql),
                    new DbParam("SqlCount", DbType.String, countSql),
                    new DbParam("Email", DbType.String, email)
                });

        }
        
    }

    #endregion

    #region "FileInviteQueryParams"

    public class FileInviteQueryParams
    {
        
        private WhereInt _id;
        private WhereInt _thingId;
        private WhereString _name;
        private WhereInt _createdUserId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt FileId
        {
            get => _thingId ?? (_thingId = new WhereInt());
            set => _thingId = value;
        }
        
        public WhereString Email
        {
            get => _name ?? (_name = new WhereString());
            set => _name = value;
        }

        public WhereInt CreatedUserId
        {
            get => _createdUserId ?? (_createdUserId = new WhereInt());
            set => _createdUserId = value;
        }

    }

    #endregion

    #region "FileInviteQueryBuilder"

    public class FileInviteQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _fileInvitesTableName;
        private readonly string _usersTableName;

        private readonly FileInviteQuery _query;

        public FileInviteQueryBuilder(FileInviteQuery query)
        {
            _query = query;
            _fileInvitesTableName = GetTableNameWithPrefix("FileInvites");
            _usersTableName = GetTableNameWithPrefix("Users");

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
            sb.Append("SELECT COUNT(f.Id) FROM ")
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
            sb.Append("fi.*, ")
                .Append("u.Email, ")
                .Append("u.UserName, ")
                .Append("u.DisplayName, ")
                .Append("u.NormalizedUserName");
            return sb.ToString();

        }

        private string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_fileInvitesTableName)
                .Append(" fi WITH (nolock) LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON fi.CreatedUserId = u.Id");
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
                sb.Append(_query.Params.Id.ToSqlString("f.Id"));
            }

            // FileId
            if (_query.Params.FileId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FileId.Operator);
                sb.Append(_query.Params.FileId.ToSqlString("fi.FileId"));
            }

            // Email
            if (!String.IsNullOrEmpty(_query.Params.Email.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Email.Operator);
                sb.Append(_query.Params.Email.ToSqlString("fi.Email", "Email"));
            }

            // CreatedUserId
            if (_query.Params.CreatedUserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CreatedUserId.Operator);
                sb.Append(_query.Params.CreatedUserId.ToSqlString("f.CreatedUserId"));
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
