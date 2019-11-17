using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{

    #region "UserLoginQuery"

    public class UserLoginQuery : DefaultQuery<UserLogin>
    {

        private readonly IStore<UserLogin> _store;

        public UserLoginQuery(IStore<UserLogin> store)
        {
            _store = store;
        }

        public UserLoginQueryParams Params { get; set; }

        public override IQuery<UserLogin> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (UserLoginQueryParams)Convert.ChangeType(defaultParams, typeof(UserLoginQueryParams));
            return this;
        }

        public override async Task<IPagedResults<UserLogin>> ToList()
        {

            var builder = new UserLoginQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();            
            var loginProvider = Params.LoginProvider.Value ?? string.Empty;
            var providerKey = Params.ProviderKey.Value ?? string.Empty;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("LoginProvider", DbType.String, loginProvider),
                new DbParam("ProviderKey", DbType.String, providerKey)
            });

        }

    }

    #endregion

    #region "UserLoginQueryParams"

    public class UserLoginQueryParams
    {

        private WhereInt _id;
        private WhereString _loginProvider;
        private WhereString _providerKey;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString LoginProvider
        {
            get => _loginProvider ?? (_loginProvider = new WhereString());
            set => _loginProvider = value;
        }

        public WhereString ProviderKey
        {
            get => _providerKey ?? (_providerKey = new WhereString());
            set => _providerKey = value;
        }

    }

    #endregion

    #region "UserLoginQueryBuilder"

    public class UserLoginQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _tableName;
        private const string TableName = "UserLogins";

        private readonly UserLoginQuery _query;

        public UserLoginQueryBuilder(UserLoginQuery query)
        {
            _query = query;
            _tableName = !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + TableName
                : TableName;
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var tablePrefix = _query.Options.TablePrefix;
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM ").Append(_tableName);
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
            }

            if (!string.IsNullOrEmpty(_query.Params.LoginProvider.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.LoginProvider.Operator);
                sb.Append(_query.Params.LoginProvider.ToSqlString("LoginProvider"));
            }

            if (!string.IsNullOrEmpty(_query.Params.ProviderKey.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ProviderKey.Operator);
                sb.Append(_query.Params.ProviderKey.ToSqlString("ProviderKey"));
            }

            return sb.ToString();
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(sortColumn.Key);
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
