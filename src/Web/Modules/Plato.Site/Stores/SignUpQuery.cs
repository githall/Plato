using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Site.Models;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace Plato.Site.Stores
{

    //#region "SignUpQuery"

    //public class SignUpQuery : DefaultQuery<SignUp>
    //{

    //    private readonly IQueryableStore<SignUp> _store;

    //    public SignUpQuery(IQueryableStore<SignUp> store)
    //    {
    //        _store = store;
    //    }

    //    public SignUpQueryParams Params { get; set; }

    //    public override IQuery<SignUp> Select<T>(Action<T> configure)
    //    {
    //        var defaultParams = new T();
    //        configure(defaultParams);
    //        Params = (SignUpQueryParams)Convert.ChangeType(defaultParams, typeof(SignUpQueryParams));
    //        return this;
    //    }

    //    public override async Task<IPagedResults<Models.SignUp>> ToList()
    //    {

    //        var builder = new SignUpQueryBuilder(this);
    //        var populateSql = builder.BuildSqlPopulate();
    //        var countSql = builder.BuildSqlCount();
    //        var email = Params.Email.Value ?? string.Empty;
    //        var companyName = Params.CompanyName.Value ?? string.Empty;
    //        var companyNameAlias = Params.CompanyNameAlias.Value ?? string.Empty;

    //        return await _store.SelectAsync(new[]
    //        {
    //            new DbParam("PageIndex", DbType.Int32, PageIndex),
    //            new DbParam("PageSize", DbType.Int32, PageSize),
    //            new DbParam("SqlPopulate", DbType.String, populateSql),
    //            new DbParam("SqlCount", DbType.String, countSql),
    //            new DbParam("Email", DbType.String, email),
    //            new DbParam("CompanyName", DbType.String, companyName),
    //            new DbParam("CompanyNameAlias", DbType.String, companyNameAlias)
    //        });

    //    }
        
    //}

    //#endregion

    //#region "SignUpQueryParams"

    //public class SignUpQueryParams
    //{
        
    //    private WhereInt _id;
    //    private WhereString _email;
    //    private WhereString _companyName;
    //    private WhereString _companyNameAlias;

    //    public WhereInt Id
    //    {
    //        get => _id ?? (_id = new WhereInt());
    //        set => _id = value;
    //    }

    //    public WhereString Email
    //    {
    //        get => _email ?? (_email = new WhereString());
    //        set => _email = value;
    //    }
    //    public WhereString CompanyName
    //    {
    //        get => _companyName ?? (_companyName = new WhereString());
    //        set => _companyName = value;
    //    }

    //    public WhereString CompanyNameAlias
    //    {
    //        get => _companyNameAlias ?? (_companyNameAlias = new WhereString());
    //        set => _companyNameAlias = value;
    //    }

    //}

    //#endregion

    //#region "SignUpQueryBuilder"

    //public class SignUpQueryBuilder : IQueryBuilder
    //{

    //    #region "Constructor"

    //    private readonly string _signUpsTableName;

    //    private readonly SignUpQuery _query;

    //    public SignUpQueryBuilder(SignUpQuery query)
    //    {
    //        _query = query;
    //        _signUpsTableName = GetTableNameWithPrefix("SignUps");
    //    }

    //    #endregion

    //    #region "Implementation"

    //    public string BuildSqlPopulate()
    //    {
    //        var whereClause = BuildWhereClause();
    //        var orderBy = BuildOrderBy();
    //        var sb = new StringBuilder();
    //        sb.Append("SELECT ")
    //            .Append(BuildPopulateSelect())
    //            .Append(" FROM ")
    //            .Append(BuildTables());
    //        if (!string.IsNullOrEmpty(whereClause))
    //            sb.Append(" WHERE (").Append(whereClause).Append(")");
    //        // Order only if we have something to order by
    //        sb.Append(" ORDER BY ").Append(!string.IsNullOrEmpty(orderBy)
    //            ? orderBy
    //            : "(SELECT NULL)");
    //        // Limit results only if we have a specific page size
    //        if (!_query.IsDefaultPageSize)
    //            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
    //        return sb.ToString();
    //    }

    //    public string BuildSqlCount()
    //    {
    //        if (!_query.CountTotal)
    //            return string.Empty;
    //        var whereClause = BuildWhereClause();
    //        var sb = new StringBuilder();
    //        sb.Append("SELECT COUNT(su.Id) FROM ")
    //            .Append(BuildTables());
    //        if (!string.IsNullOrEmpty(whereClause))
    //            sb.Append(" WHERE (").Append(whereClause).Append(")");
    //        return sb.ToString();
    //    }

    //    #endregion

    //    #region "Private Methods"

    //    private string BuildPopulateSelect()
    //    {
    //        var sb = new StringBuilder();
    //        sb.Append("*");
    //        return sb.ToString();
    //    }

    //    private string BuildTables()
    //    {
    //        var sb = new StringBuilder();
    //        sb.Append(_signUpsTableName)
    //            .Append(" su ");
    //        return sb.ToString();
    //    }

    //    private string GetTableNameWithPrefix(string tableName)
    //    {
    //        return !string.IsNullOrEmpty(_query.Options.TablePrefix)
    //            ? _query.Options.TablePrefix + tableName
    //            : tableName;
    //    }

    //    private string BuildWhereClause()
    //    {
    //        var sb = new StringBuilder();

    //        // Id
    //        if (_query.Params.Id.Value > -1)
    //        {
    //            if (!string.IsNullOrEmpty(sb.ToString()))
    //                sb.Append(_query.Params.Id.Operator);
    //            sb.Append(_query.Params.Id.ToSqlString("su.Id"));
    //        }

    //        // Email
    //        if (!string.IsNullOrEmpty(_query.Params.Email.Value))
    //        {
    //            if (!string.IsNullOrEmpty(sb.ToString()))
    //                sb.Append(_query.Params.Email.Operator);
    //            sb.Append(_query.Params.Email.ToSqlString("Email"));
    //        }

    //        // CompanyName
    //        if (!string.IsNullOrEmpty(_query.Params.CompanyName.Value))
    //        {
    //            if (!string.IsNullOrEmpty(sb.ToString()))
    //                sb.Append(_query.Params.CompanyName.Operator);
    //            sb.Append(_query.Params.CompanyName.ToSqlString("CompanyName"));
    //        }

    //        // CompanyNameAlias
    //        if (!string.IsNullOrEmpty(_query.Params.CompanyNameAlias.Value))
    //        {
    //            if (!string.IsNullOrEmpty(sb.ToString()))
    //                sb.Append(_query.Params.CompanyNameAlias.Operator);
    //            sb.Append(_query.Params.CompanyNameAlias.ToSqlString("CompanyNameAlias"));
    //        }

    //        return sb.ToString();

    //    }

    //    private string GetQualifiedColumnName(string columnName)
    //    {
    //        if (columnName == null)
    //        {
    //            throw new ArgumentNullException(nameof(columnName));
    //        }

    //        return columnName.IndexOf('.') >= 0
    //            ? columnName
    //            : "su." + columnName;
    //    }

    //    private string BuildOrderBy()
    //    {
    //        if (_query.SortColumns.Count == 0) return null;
    //        var sb = new StringBuilder();
    //        var i = 0;
    //        foreach (var sortColumn in _query.SortColumns)
    //        {
    //            sb.Append(GetQualifiedColumnName(sortColumn.Key));
    //            if (sortColumn.Value != OrderBy.Asc)
    //                sb.Append(" DESC");
    //            if (i < _query.SortColumns.Count - 1)
    //                sb.Append(", ");
    //            i += 1;
    //        }
    //        return sb.ToString();
    //    }

    //    #endregion

    //}

    //#endregion

}
