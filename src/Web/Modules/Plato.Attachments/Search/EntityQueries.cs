using System;
using System.Text;
using System.Collections.Generic;
using Plato.Attachments.Stores;
using PlatoCore.Data.Abstractions;
using PlatoCore.Search.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;

namespace Plato.Attachments.Search
{

    public class AttachmentQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public AttachmentQueries(IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> Build(IQuery<TModel> query)
        {

            // Ensure correct query type for federated query
            if (query.GetType() != typeof(AttachmentQuery))
            {
                return null;
            }
            
            // Convert to correct query type
            var typedQuery = (AttachmentQuery)Convert.ChangeType(query, typeof(AttachmentQuery));
            
            return query.Options.SearchType != SearchTypes.Tsql
                ? BuildFullTextQueries(typedQuery)
                : BuildSqlQueries(typedQuery);
        }

        // ----------

        IList<string> BuildSqlQueries(AttachmentQuery query)
        {
            
            // Entities
            // ----------------------

            var q1 = new StringBuilder();
            q1.Append("SELECT a.Id, 0 AS [Rank] FROM ")
                .Append("{prefix}_Attachments")
                .Append(" a WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(query.Params.Keywords.ToSqlString("a.[Name]", "Keywords"))
                .Append(" OR ")
                .Append(query.Params.Keywords.ToSqlString("a.Extension", "Keywords"))
                .Append("));");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };
            
        }

        IList<string> BuildFullTextQueries(AttachmentQuery query)
        {

            // Parse keywords into valid full text query syntax
            var fullTextQuery = _fullTextQueryParser.ToFullTextSearchQuery(query.Params.Keywords.Value);

            // Ensure parse was successful
            if (!String.IsNullOrEmpty(fullTextQuery))
            {
                fullTextQuery = fullTextQuery.Replace("'", "''");
            }

            // Can be empty if only puntutaton or stop words were entered
            if (string.IsNullOrEmpty(fullTextQuery))
            {
                return null;
            }
            
            var q1 = new StringBuilder();
            q1
                .Append("SELECT i.[Key], i.[Rank] ")
                .Append("FROM ")
                .Append("{prefix}_Attachments a INNER JOIN ")
                .Append(query.Options.SearchType.ToString().ToUpper())
                .Append("({prefix}_Attachments")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (query.Options.MaxResults > 0)
                q1.Append(", ").Append(query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = a.Id WHERE ");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(a.Id IN (IsNull(i.[Key], 0)));");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

    }

}
