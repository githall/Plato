using System;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Search.Models
{

    public class FullTextCatalog : IDbModel<FullTextCatalog>
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public void PopulateModel(IDataReader dr)
        {
            if (dr.ColumnIsNotNull("fulltext_catalog_id"))
                Id = Convert.ToInt32(dr["fulltext_catalog_id"]);

            if (dr.ColumnIsNotNull("name"))
                Name = Convert.ToString(dr["name"]);
            
        }

    }

}
