using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Stores;
using PlatoCore.Data.Abstractions;
using PlatoCore.Search.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;

namespace Plato.Entities.Attachments.Search
{

    public class FeatureEntityCountQueries<TModel> : IFederatedQueryProvider<TModel> where TModel : class
    {

        protected readonly IFullTextQueryParser _fullTextQueryParser;

        public FeatureEntityCountQueries(IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
        }

        public IEnumerable<string> Build(IQuery<TModel> query)
        {

            // Ensure correct query type for federated query
            if (query.GetType() != typeof(FeatureEntityCountQuery<TModel>))
            {
                return null;
            }

            // Convert to correct query type
            var entityQuery = (FeatureEntityCountQuery<TModel>)Convert.ChangeType(query, typeof(FeatureEntityCountQuery<TModel>));

            return query.Options.SearchType != SearchTypes.Tsql
                ? BuildFullTextQueries(entityQuery)
                : BuildSqlQueries(entityQuery);
        }

        List<string> BuildSqlQueries(FeatureEntityCountQuery<TModel> query)
        {

            /*
                 Produces the following federated query...
                 -----------------
                 SELECT ea.EntityId, 0 FROM plato_Attachments a
                 INNER JOIN plato_EntityAttachments ea ON ea.AttachmentId = a.Id
                 INNER JOIN plato_Entities e ON e.Id = ea.EntityId
                 WHERE (a.[Name] LIKE '%percent') GROUP BY ea.EntityId;     
             */

            var q1 = new StringBuilder();
            q1.Append("SELECT ea.EntityId, 0 FROM {prefix}_Attachments a ")
                .Append("INNER JOIN {prefix}_EntityAttachments ea ON ea.AttachmentId = a.Id ")
                .Append("INNER JOIN {prefix}_Entities e ON e.Id = ea.EntityId ")
                .Append("WHERE (");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(")
                .Append(query.Params.Keywords.ToSqlString("l.[Name]", "Keywords"))
                .Append(" OR ")
                .Append(query.Params.Keywords.ToSqlString("l.Extension", "Keywords"))
                .Append("));");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

        List<string> BuildFullTextQueries(FeatureEntityCountQuery<TModel> query)
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

            /*
                Produces the following federated query...
                -----------------
                SELECT ea.EntityId, SUM(i.[Rank]) AS [Rank] 
                FROM plato_Attachments a INNER JOIN 
                CONTAINSTABLE(plato_Attachments, *, 'FORMSOF(INFLECTIONAL, creative)') AS i ON i.[Key] = l.Id 
                INNER JOIN plato_EntityAttachments ea ON ea.AttachmentId = a.Id
                INNER JOIN plato_Entities e ON e.Id = ea.EntityId
                WHERE (a.Id IN (IsNull(i.[Key], 0))) GROUP BY ea.EntityId;
             */

            var q1 = new StringBuilder();
            q1
                .Append("SELECT ea.EntityId, SUM(i.[Rank]) ")
                .Append("FROM ")
                .Append("{prefix}_Attachments")
                .Append(" a ")
                .Append("INNER JOIN ")
                .Append(query.Options.SearchType.ToString().ToUpper())
                .Append("(")
                .Append("{prefix}_Attachments")
                .Append(", *, '").Append(fullTextQuery).Append("'");
            if (query.Options.MaxResults > 0)
                q1.Append(", ").Append(query.Options.MaxResults.ToString());
            q1.Append(") AS i ON i.[Key] = a.Id ")
                .Append("INNER JOIN {prefix}_EntityAttachments ea ON ea.AttachmentId = a.Id ")
                .Append("INNER JOIN {prefix}_Entities e ON e.Id = ea.EntityId ")
                .Append("WHERE ");
            if (!string.IsNullOrEmpty(query.Builder.Where))
            {
                q1.Append("(").Append(query.Builder.Where).Append(") AND ");
            }
            q1.Append("(a.Id IN (IsNull(i.[Key], 0))) GROUP BY ea.EntityId;");

            // Return queries
            return new List<string>()
            {
                q1.ToString()
            };

        }

    }

}
