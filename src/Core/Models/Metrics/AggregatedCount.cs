using System;
using System.Data;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;

namespace PlatoCore.Models.Metrics
{

    public class AggregatedCount<T> : IDbModel
    {

        public T Aggregate { get; set; }

        public int Count { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Aggregate"))
                Aggregate = (T)(dr["Aggregate"]);

            if (dr.ColumnIsNotNull("Count"))
                Count = Convert.ToInt32(dr["Count"]);

        }

    }

}
