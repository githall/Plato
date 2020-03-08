using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;
using Plato.Entities.Attachments.Models;

namespace Plato.Entities.Attachments.Stores
{

    #region "EntityAttachmentQuery"

    public class EntityAttachmentQuery : DefaultQuery<EntityAttachment>
    {

        private readonly IQueryableStore<EntityAttachment> _store;

        public EntityAttachmentQuery(IQueryableStore<EntityAttachment> store)
        {
            _store = store;
        }

        public EntityAttachmentQueryParams Params { get; set; }

        public override IQuery<EntityAttachment> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityAttachmentQueryParams)Convert.ChangeType(defaultParams, typeof(EntityAttachmentQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityAttachment>> ToList()
        {

            var builder = new EntityAttachmentQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var attachmentId = Params.AttachmentId.Value;
            var entityId = Params.EntityId.Value;

            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("AttachmentId", DbType.Int32, attachmentId),
                new DbParam("EntityId", DbType.Int32, entityId)
            });

        }

    }

    #endregion

    #region "EntityAttachmentQueryParams"

    public class EntityAttachmentQueryParams
    {

        private WhereInt _id;
        private WhereInt _attachmentId;
        private WhereInt _entityId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt AttachmentId
        {
            get => _attachmentId ?? (_attachmentId = new WhereInt());
            set => _attachmentId = value;
        }

        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }

    }

    #endregion

    #region "EntityAttachmentQueryBuilder"

    public class EntityAttachmentQueryBuilder : IQueryBuilder
    {

        #region "Constructor"
  
        private readonly string _entityAttachmentsTableName;
        private readonly string _shellFeaturesTableName;
        private readonly string _attachmentsTableName;
        private readonly string _usersTableName;

        private readonly EntityAttachmentQuery _query;

        public EntityAttachmentQueryBuilder(EntityAttachmentQuery query)
        {
            _query = query;            
            _entityAttachmentsTableName = GetTableNameWithPrefix("EntityAttachments");            
            _shellFeaturesTableName = GetTableNameWithPrefix("ShellFeatures");
            _attachmentsTableName = GetTableNameWithPrefix("Attachments");
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
            sb.Append("SELECT COUNT(ea.Id) FROM ")
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
                .Append("ea.*, ")   
                .Append("a.FeatureId, ")
                .Append("a.[Name], ")
                .Append("CAST(1 AS BINARY(1)) AS ContentBlob, ") // for perf not returned with paged results
                .Append("a.ContentType, ")
                .Append("a.ContentLength, ")
                .Append("a.ContentGuid, ")
                .Append("a.ContentCheckSum, ")
                .Append("a.TotalViews, ")
                .Append("f.ModuleId, ")
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
            sb.Append(_entityAttachmentsTableName)
                .Append(" ea ");
                
            // join attachments
            sb.Append("INNER JOIN ")
                .Append(_attachmentsTableName)
                .Append(" a ON ea.AttachmentId = a.Id ");

            // join shell features
            sb.Append("LEFT OUTER JOIN ")
                .Append(_shellFeaturesTableName)
                .Append(" f ON a.FeatureId = f.Id ");

            // join created user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" c ON a.CreatedUserId = c.Id ");

            // join last modified user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" m ON a.ModifiedUserId = m.Id ");

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
                sb.Append(_query.Params.Id.ToSqlString("ea.Id"));
            }
            
            // LabelId
            if (_query.Params.AttachmentId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.AttachmentId.Operator);
                sb.Append(_query.Params.AttachmentId.ToSqlString("ea.AttachmentId"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("ea.EntityId"));
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
                : "el." + columnName;
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
