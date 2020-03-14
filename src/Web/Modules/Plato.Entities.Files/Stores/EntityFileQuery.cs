using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;
using Plato.Entities.Files.Models;

namespace Plato.Entities.Files.Stores
{

    #region "EntityFileQuery"

    public class EntityFileQuery : DefaultQuery<EntityFile>
    {

        private readonly IQueryableStore<EntityFile> _store;

        public EntityFileQuery(IQueryableStore<EntityFile> store)
        {
            _store = store;
        }

        public EntityFileQueryParams Params { get; set; }

        public override IQuery<EntityFile> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityFileQueryParams)Convert.ChangeType(defaultParams, typeof(EntityFileQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityFile>> ToList()
        {

            var builder = new EntityFileQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var fileId = Params.FileId.Value;
            var entityId = Params.EntityId.Value;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("FileId", DbType.Int32, fileId),
                new DbParam("EntityId", DbType.Int32, entityId)
            });

        }

    }

    #endregion

    #region "EntityFileQueryParams"

    public class EntityFileQueryParams
    {

        private WhereInt _id;
        private WhereInt _fileId;
        private WhereInt _entityId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt FileId
        {
            get => _fileId ?? (_fileId = new WhereInt());
            set => _fileId = value;
        }

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }

    }

    #endregion

    #region "EntityFileQueryBuilder"

    public class EntityFileQueryBuilder : IQueryBuilder
    {

        #region "Constructor"
  
        private readonly string _entityFilesTableName;
        private readonly string _shellFeaturesTableName;
        private readonly string _filesTableName;
        private readonly string _usersTableName;

        private readonly EntityFileQuery _query;

        public EntityFileQueryBuilder(EntityFileQuery query)
        {
            _query = query;            
            _entityFilesTableName = GetTableNameWithPrefix("EntityFiles");            
            _shellFeaturesTableName = GetTableNameWithPrefix("ShellFeatures");
            _filesTableName = GetTableNameWithPrefix("Files");
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
            sb.Append("SELECT COUNT(ef.Id) FROM ")
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
                .Append("ef.*, ")   
                .Append("f.FeatureId, ")
                .Append("f.[Name], ")
                .Append("f.Alias, ")
                .Append("f.Extension, ")
                .Append("CAST(1 AS BINARY(1)) AS ContentBlob, ") // for perf not returned with paged results
                .Append("f.ContentType, ")
                .Append("f.ContentLength, ")
                .Append("f.ContentGuid, ")
                .Append("f.ContentCheckSum, ")
                .Append("f.TotalViews, ")
                .Append("sf.ModuleId, ")
                .Append("c.UserName AS CreatedUserName, ")
                .Append("c.DisplayName AS CreatedDisplayName, ")
                .Append("c.Alias AS CreatedAlias, ")
                .Append("c.PhotoUrl AS CreatedPhotoUrl, ")
                .Append("c.PhotoColor AS CreatedPhotoColor, ")
                .Append("c.SignatureHtml AS CreatedSignatureHtml, ")
                .Append("c.IsVerified AS CreatedIsVerified, ")
                .Append("c.IsStaff AS CreatedIsStaff, ")
                .Append("c.IsSpam AS CreatedIsSpam, ")
                .Append("c.IsBanned AS CreatedIsBanned, ")
                .Append("m.UserName AS ModifiedUserName, ")
                .Append("m.DisplayName AS ModifiedDisplayName, ")
                .Append("m.Alias AS ModifiedAlias, ")
                .Append("m.PhotoUrl AS ModifiedPhotoUrl, ")
                .Append("m.PhotoColor AS ModifiedPhotoColor, ")
                .Append("m.SignatureHtml AS ModifiedSignatureHtml, ")
                .Append("m.IsVerified AS ModifiedIsVerified, ")
                .Append("m.IsStaff AS ModifiedIsStaff, ")
                .Append("m.IsSpam AS ModifiedIsSpam, ")
                .Append("m.IsBanned AS ModifiedIsBanned");
            return sb.ToString();

        }

        private string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_entityFilesTableName)
                .Append(" ef ");
                
            // join files
            sb.Append("INNER JOIN ")
                .Append(_filesTableName)
                .Append(" f ON ef.FileId = f.Id ");

            // join shell features
            sb.Append("LEFT OUTER JOIN ")
                .Append(_shellFeaturesTableName)
                .Append(" sf ON f.FeatureId = sf.Id ");

            // join created user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON f.CreatedUserId = c.Id ");

            // join last modified user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON f.ModifiedUserId = m.Id ");

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
            if (_query.Params.Id.Value > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("ef.Id"));
            }
            
            // LabelId
            if (_query.Params.FileId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.FileId.Operator);
                sb.Append(_query.Params.FileId.ToSqlString("ef.FileId"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("ef.EntityId"));
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
                : "ef." + columnName;
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
